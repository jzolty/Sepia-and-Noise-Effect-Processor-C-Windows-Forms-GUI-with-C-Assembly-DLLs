; _______________________________________________________________________________________________________
;	Autor: Julia ¯ó³ty
;   Projekt: Jêzyki Asemblerowe
;   Tytu³: efekt sepii z efektem szumi(postarzania)
;   Sekcja : 12
;   Data obrony projektu: 30.01.2025
; 
;   Plik biblioteki DLL. Zawiera funkcjê do konwersji obrazu kolorowego na sepiê oraz szum napisan¹ w asemblerze.
;   Argumenty:
; rcx = pixelBuffer - wskaŸnik na buffor przechowujacy piksele obrazu
; rdx = width - szerokoœæ obrazu
; r8 = bytesPerPixel - iloœæ bajtów w pikselu (3)
; r9 = stride - iloœæ bajtów na wiersz
; P = [rsp + 28h] - parametr P (sepii)
; startRow = [rsp + 30h] - pocz¹tek wiersza
; endRow = [rsp + 38h] - koñcowy wiersz
; X = [rsp +40h] - paramert X (szum)
; _______________________________________________________________________________________________________


.data
;Wagi podane w formacie IEEE-754 rozumianym przez asm zeby unikac dodatkowego pakowania/rozpakowywania floatow, dd rezerwuje 4 bajty w pamiêci i zapisuje tam wartoœæ 32-bitow¹.
weight_blue dd 1038710997  ; 0.114f - Waga B
weight_green dd 1058424226  ; 0.587f - Waga G
weight_red dd 1050220167  ; 0.299f - Waga R
 
;Mnozniki parametru P (BGR), do 16 bajtow wyrownane 
p_multipliers db 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 0, 0, 0
 
;Operuje jednoczesnie na 4 pikselach poniewa¿ RGB jest wyrownane 24 lub 32 bajtow wiec -> nie mozna zalozyc ze bedzie podzielne przez 16 ale bedzie przez 12
;Wszystkie maski sa wyrownane do 16 bajtow - rejestry xmm to rejestry 16-bajtowe
 
;MASKI DO EKSPORTU KANALOW B G R (bufor wziety z bitmapy zatem jest w odwroconej kolejnosci, zamiast RGB)
 
;Eksport kanalu B (bez utraty czegokolwiek 80h) 
blue_channel db 0, 80h, 80h, 80h, 3, 80h, 80h, 80h, 6, 80h, 80h, 80h, 9, 80h, 80h, 80h
;Maska po nalozeniu: B1 0 0 0 B2 0 0 0 B3 0 0 0 B4 0 0 0
;Kopiowane bity: bit0 0 0 0 bit3 0 0 0 bit6 0 0 0 bit9 0 0 0
 
;Eksport kanalu G
green_channel db 1, 80h, 80h, 80h, 4, 80h, 80h, 80h, 7, 80h, 80h, 80h, 10, 80h, 80h, 80h
;Maska po nalozeniu: G1 0 0 0 G2 0 0 0 G3 0 0 0 G4 0 0 0
;Kopiowane bity: bit1 0 0 0 bit4 0 0 0 bit7 0 0 0 bit10 0 0 0

;Eksport kanalu R
red_channel db 2, 80h, 80h, 80h, 5, 80h, 80h, 80h, 8, 80h, 80h, 80h, 11, 80h, 80h, 80h
;Maska po nalozeniu: R1 0 0 0 R2 0 0 0 R3 0 0 0 R4 0 0 0
;Kopiowane bity: bit2 0 0 0 bit5 0 0 0 bit8 0 0 0 bit11 0 0 0
 
 
;MASKI DO ZAPISU KANALOW B G R
 
;Niezaleznie od maski zapisujemy tylko bity 0, 4, 8 i 12, daje nam to mozliwosc powielenia od razu szarosci, ostatnie 4 bajty sa nieuzywane, wiec sa zerowane
;Zerowane w celu aby wyrownac do 12 bajtow (szarosc 4 pikseli aby potem nie dodawalo zadnych dodatkowych warosci)
 
;Zapis kanalu B
blue_save db 0, 80h, 80h, 4, 80h, 80h, 8, 80h, 80h, 12, 80h, 80h, 80h, 80h, 80h, 80h
;Maska po nalozeniu: gray1 0 0 gray2 0 0 gray3 0 0 gray4 0 0 0 0 0 0
;Kopiowane bity: bit0 0 0 bit4 0 0 bit8 0 0 bit12 0 0 0 0 0 0

green_save db 80h, 0, 80h, 80h, 4, 80h, 80h, 8, 80h, 80h, 12, 80h, 80h, 80h, 80h, 80h
;Maska po nalozeniu: 0 gray1 0 0 gray2 0 0 gray3 0 0 gray4 0 0 0 0 0
;Kopiowane bity: 0 bit0 0 0 bit4 0 0 bit8 0 0 bit12 0 0 0 0 0 

red_save db 80h, 80h, 0, 80h, 80h, 4, 80h, 80h, 8, 80h, 80h, 12, 80h, 80h, 80h, 80h
;Maska po nalozeniu: 0 0 gray1 0 0 gray2 0 0 gray3 0 0 gray4 0 0 0 0
;Kopiowane bity: 0 0 bit0 0 0 bit4 0 0 bit8 0 0 bit12 0 0 0 0
 

rng_values dd 0, 0, 0, 0 ; Tymczasowa tablica przechowujaca dane RNG, 4 piksele
 
.code
ApplySepiaFilterAsm PROC

; Przygotowanie danych. Inicjalizacja zmiennych przekazanych jako argumenty funkcji
    mov r10d, dword ptr [rsp + 30h] ; startRow -> r10d  ;r10 64bitowy, r10d 32bitowy
    mov r11d, dword ptr [rsp + 38h] ; endRow -> r11d
 
    mov r14d, dword ptr [rsp + 40h] ; X -> r14d
 
    mov rbx, rcx ; rcx -> rbx  ; kopiuje rcx do rbx, przechowywanie wskaŸnika bufora pikseli w rbx
 
 ; £adowanie sta³ych do rejestrów SIMD
    movdqu xmm5, xmmword ptr [p_multipliers] ; kopiuje mno¿niki dla P do xmm5, p_multipliers -> xmm5
    movd xmm6, dword ptr [rsp + 28h] ; P -> xmm6
    pshufd xmm6, xmm6, 0 ; powielenie P na calym rejestrze xmm6
    
    pmulld xmm6, xmm5 ; przemnozenie P przez p_multipliers
 
  ; £adowanie wag do rejestrów SIMD
  ; Instrukcja z zestawu AVX vbroadcastss wrzuca do rejestru i powiela
    vbroadcastss xmm7, weight_blue ; weight_blue -> xmm7 (+ rozprowadzenie)
    vbroadcastss xmm8, weight_green ; weight_green -> xmm8 (+ rozprowadzenie)
    vbroadcastss xmm9, weight_red ; weight_red -> xmm9 (+ rozprowadzenie)
 
 ; £adowanie masek i masek do zapisu do rejestrów SIMD
    movdqu xmm10, xmmword ptr [blue_channel] ; blue_channel -> xmm10
    movdqu xmm11, xmmword ptr [green_channel] ; green_channel -> xmm11
    movdqu xmm12, xmmword ptr [red_channel] ; red_channel -> xmm12
 
 ;Zabezpiieczenie przez nakladaniem dodatkowych niepotrzebych rzeczy
    movdqu xmm13, xmmword ptr [blue_save] ; blue_save -> xmm13
    movdqu xmm14, xmmword ptr [green_save] ; green_save -> xmm14
    movdqu xmm15, xmmword ptr [red_save] ; red_save -> xmm15
 
RowLoop: ;Petla do wierszy
    cmp r10d, r11d ; Jesli obecny wiersz odpowiada koncowemu, porownuje endRow i startRow
    jge EndProc    ; skocz do zakonczenia procesu, jump if  greater or equal
 
    mov rcx, rbx ; rbx -> rcx przywracam wskaznik buforu pikseli rcx z rbx
    mov eax, r10d ; kopiuje obecny wiersz -> eax  
    imul eax, r9d ; eax * bytesPerPixel (3)
    add rcx, rax ; ecx -> eax (przejscie do wiersza ktory ma byc nastepny)
 
    xor r13, r13 ; zerowanie licznika pikseli
 
PixelLoop: ;Petla do pikseli
    cmp r13d, edx ; jesli licznik pikseli odpowiada szerokosci obrazu
    jge NextRow   ; skocz do przejscia do nastepnego wiersza
 
    jmp NoiseGenerator ; Skok bezwarunkowy do generatora szumu
 
NG_Return:
;£adujemy 4 piksele (8+4 bajty) w 2 krokach, xmm max 16 bajtow
    movlps xmm0, qword ptr [rcx] ; ladowanie pierwszych 64 bitow (8 bajtow) spod wskaznika rcx do gornej czesci xmm0  (2 2/3 piksela)
    movd xmm1, dword ptr [rcx+8] ; ladowanie kolejnych 32 bitow (4 bajtow) spod wskaznika rcx do dolnej czesci xmm1   (1 1/3 piksela)
    ;movd , semiwektorowy
    movlhps xmm0, xmm1 ; laczenie obu rejestrow - kopiowanie 32 bitow z dolnej czesci xmm1 na najblizsze wolne miejsca w gornej czesci xmm0, zeby bylo bez nadpisania
 
    movdqa xmm2, xmm0 ; kopiowanie zawartosci xmm0 do xmm2
    pshufb xmm2, xmm10 ; wyciagniecie kanalu B
    mulps xmm2, xmm7 ; mnozenie przez wage B
 
    movdqa xmm3, xmm0 ; kopiowanie zawartosci xmm0 do xmm3
    pshufb xmm3, xmm11 ; wyciagniecie kanalu G
    mulps xmm3, xmm8 ; mnizenie przez wage G
 
    movdqa xmm4, xmm0 ; kopiowanie zawartosci xmm0 do xmm4
    pshufb xmm4, xmm12 ; wyciagniecie kanalu R
    mulps xmm4, xmm9 ; mnozenie przez wage R
 
    paddd xmm2, xmm3 ; B + G [dodawanie 32-bitowe czyli doublewordy], asm skonwertowal do wartosci 32 bit
    paddd xmm2, xmm4 ; (B + G) + R [dod. 32-bitowe]
    ;xmm2 = gray1 0 0 0 gray2 0 0 0 gray3 0 0 0 gray4 0 0 0, otrzymalismy szarosc
 
    paddd xmm2, xmm5 ; Dodanie szumu
 
    ;ograniczenie skali wartosci piksela 0-255
    pxor xmm0, xmm0 ; Zerowanie xmm0
    pcmpeqd xmm1, xmm1 ; Wszystkie bity xmm1 na 1
    psrld xmm1, 24 ; Przesuniecie bitow o 24 w lewo - tworzy liczbe 255
 
    pmaxsd xmm2, xmm0 ; Clamp 0 , ograniczenie wartosci minimalne
    pminsd xmm2, xmm1 ; Clamp 255 , ograniczenie wartosci maksymalnej
 
    ;rozpropagowanie szarosci na kanaly
    movdqa xmm0, xmm2 ; kopiowanie xmm2 sumy kanalow do xmm0
    pshufb xmm0, xmm13 ; wyodrebnienie kanalu B do zapisu, z maska do zapisu
 
    movdqa xmm1, xmm2 ; kopiowanie sumy kanalow do xmm1
    pshufb xmm1, xmm14 ; wyodrebnianie kanalu G do zapisu
 
    movdqa xmm3, xmm2 ; kopiowanie sumy kanalow R do xmm3
    pshufb xmm3, xmm15 ; wyodrebnianie kanalu R do zapisu
 
    paddb xmm3, xmm1 ; laczenie xmm1 z xmm3 (kanaly R i G) [dod. 8-bitowe], dodajemy bajty a nie inty
    paddb xmm3, xmm0 ; laczenie xmm0 z xmm3 (kanaly R, G i B) [dod. 8-bitowe]
    ; xmm3 = gray1 gray1 gray1 gray2 gray2 gray2 gray3 gray3 gray3 gray4 gray4 gray4 0 0 0 0
 
    paddusb xmm3, xmm6 ; dodawanie  parametru P (0, P, 2P) do wartoœci szaroœci
    ;"u" ogranicza ze w zakresie 0-255

    ;Zapisanie
    movq qword ptr [rcx], xmm3 ; zapisanie (xmm3)pierwszych 64 bitow=8bajtow=2i2/3piksela, qworda, pod wskaxnik rcx czyli nadpisujemy dane
    movhlps xmm3, xmm3 ; przeniesienie najwczeœniej dolnych zajetych bitow xmm3 na poczatek rejestru
    movd dword ptr [rcx + 8], xmm3 ; zapisanie pozostalych 32 bitow=4bajtow=1i1/3piksela
 
    mov r12, 4 ; r12 ustawiam na 4
    imul r12, r8 ; r12 * bytesPerPixel(czyli 3)
    add rcx, r12 ; r12 dodaje do rcx (wskaznika)
    add r13d, 4 ; r13 += 4, inkrementacja licznika pikseli o 4
    jmp PixelLoop ; Skok na poczatek petli
 
NextRow: ;petlna skoku do nastepnego wiersza
    inc r10d ; r10++ (startRow)
    jmp RowLoop ; Skok do petli wierszy
 
EndProc:
    ret
 
NoiseGenerator:
    ; ZABEZPIECZENIE REJESTROW NA STOSIE, ¿eby siê nie nadpisa³y
    push rcx
    push rax
    push rdx
    push rsi
    push r15
    push r13
 
    xor r13, r13 ; Zerowanie licznika r13 
 
 ;lea - przypisanie do rcx wskaznika ktory wskazuje na adres rng_values - tymczasowa tablica
    lea rcx, [rng_values] ; Wskaznik na rng_values
 
NG_Loop:
    mov r15d, r14d ; kopiuje X -> r15d
    shl r15d, 1 ;shift left  r15d*2 = 2X , mznozenie *2 przez przesuniecie bitowe w lewo
    add r15d, 1 ; r15d + 1 = 2X + 1 ( inc r15d) 
 
 ;Pobieranie czasu, liczba 64 bitowa rozprowadzona miedzy 2 rejestry - dolne 32bity eax i edx
 ;Sygnatura czasowa procesora rejestruje liczbê cykli zegara od ostatniego zresetowania.
    rdtsc ; Odczytanie danych z licznika taktow CPU (do rejestrow eax i edx)
 
    ;XORSHIFT dla wiekszej losowosci danych, rand() z cpp ma to wbudowane
   ;bez tego wartoœci by³y bardzo podobne, takie same
    mov esi, eax     ; przeniesienie wartoœci z rejestru eax do rejestru esi. Wartoœæ pocz¹tkowa do przekszta³cenia.
    xor esi, edx     ; xor ³¹czy bity z esi i edx, generuj¹c now¹, losow¹ wartoœæ.
    mov edx, esi     ;przeniesienie wyniku z rejestru esi do edx
    shl edx, 13      ;przesuniêcie bitów w rejestrze edx o 13 pozycji w lewo. (mno¿enie 2^13)
    xor esi, edx     ;operacja XOR miêdzy rejestrami esi i edx. Dodaje losowoœæ do obecnej wartoœci w esi.
    mov edx, esi     ;esi->edx
    shr edx, 17      ;przesuniecie bitowe w rejestrze edx o 17 w prawo (dzielenie przez 2^17)
    xor esi, edx     ;operacja XOR miêdzy rejestrami esi i edx. Dodaje losowoœæ do obecnej wartoœci w esi.
    mov edx, esi     ;esi->edx
    shl edx, 5       ;przesuniêcie bitów w rejestrze edx o 5 pozycji w lewo (mno¿enie przez 2^5)
    xor esi, edx     ;ostateczna operacja XOR miêdzy esi i edx. Wynik jest bardziej losowy, bazuj¹cy na pocz¹tkowej wartoœci.
    ; Koncowo esi zawiera nasz seed na bazie zegara CPU
 
    test esi, esi ; Sprawdzenie bitu znaku seeda, operacja AND na esi
    jns NG_Randomize ; Jesli seed dodatni, pomin negacje, jns testowanie bitu znaku 
    neg esi ; Jesli ujemny zaneguj
 
NG_Randomize:
    xor eax, eax ; Zerowanie eax
    xor edx, edx ; Zerowanie edx
    mov eax, esi ; esi (wartosc koncowa operacji xorshift)-> eax
    cdq ; Rozszerzenie 32-bit eax na 64-bit na eax + edx, po to aby u¿yæ instrukcji idiv, w r15d jest nasz X
    idiv r15d ; Dzielenie z reszta przez r15d - wynik w eax, reszta w edx(ta reszta nas interesuje), div zeruje zerszte z dzielenia 
    mov r15d, edx ; edx -> r15d
    sub r15d, r14d ; Przesuniêcie zakresu do [-X, X] przez odjêcie X
 
    mov [rcx + r13*4], r15d ; Zapisanie wygenerowanej liczby do tymczasowej tablicy rng_values, *4 bo 4 bajtowe slowa(dw, inty)
 
    inc r13 ; r13++
    cmp r13, 4 ; Dopoki r13 < 4, dopóki nie mamy 4 wartoœci 
    jl NG_Loop ; Kontynuuj petle, jl je¿eli nie
 
    xorps xmm5, xmm5 ; Zerowanie xmm5 (jakby zostaly tam jakies smieci), xor dla wektorowych
    movdqu xmm5, xmmword ptr [rng_values] ; Przeniesienie zawartosci tymczasowej tablicy do xmm5
 ;teraz w xmm5 4 wartosci dla 4 pikseli
 
    ; PRZYWROCENIE DANYCH ZE STOSU, w odwrotnej kolejnosci
    pop r13
    pop r15
    pop rsi
    pop rdx
    pop rax
    pop rcx
 
    jmp NG_Return ; Skok bezwarunkowy z powrotem do PixelLoop
 
ApplySepiaFilterAsm ENDP
END