#include "pch.h"
#include <iostream>
#include <cstdlib>   // rand()    
#include <windows.h>

//Funkcja pomocnicza do ograniczania warto�ci do zakresu [low, high]
//Je�li warto�� jest mniejsza ni� 'low', zwraca 'low', je�li wi�ksza ni� 'high', zwraca 'high'.
//W przeciwnym razie zwraca oryginaln� warto��.
inline int clamp(int value, int low, int high) {
    return (value < low) ? low : (value > high ? high : value);
}

//Eksportowana funkcja ApplySepiaFilter, kt�ra przyjmuje zakres wierszy obrazu do przetworzenia
extern "C" __declspec(dllexport) void ApplySepiaFilter(unsigned char* pixelBuffer, int width, int bytesPerPixel, int P, int X, int startRow, int endRow, int stride)
{
    // Iteracja po wierszach obrazu od startRow do endRow
    for (int y = startRow; y < endRow; y++)
    {
        //Pocz�tek bie��cego wiersza w buforze pamieci
        int rowStart = y * stride;

        // Iteracja po pikselach w bie��cym wierszu
        for (int x = 0; x < width; x++)
        {
            // Obliczenie indeksu bie��cego piksela w buforze
            int pixelIndex = rowStart + x * bytesPerPixel;

            //Pobranie kana��w RGB z bufora
            unsigned char B = pixelBuffer[pixelIndex];     //Blue
            unsigned char G = pixelBuffer[pixelIndex + 1]; //Green
            unsigned char R = pixelBuffer[pixelIndex + 2]; //Red

            //Konwersja na odcie� szaro�ci
            int gray = static_cast<int>(0.299 * R + 0.587 * G + 0.114 * B);

            // Wyliczenie nowych warto�ci kana��w dla efektu sepii
            int newB = gray;
            int newG = P + gray;
            int newR = 2 * P + gray;

            // Ograniczenie warto�ci do zakresu [0, 255]
            newB = clamp(newB, 0, 255);
            newG = clamp(newG, 0, 255);
            newR = clamp(newR, 0, 255);

            // Dodanie efektu postarzenia z parametrem X
            int noise = (rand() % (2 * X + 1)) - X; // losowa wartosc z przedzia�u [-X, X]
            newB = clamp(newB + noise, 0, 255);
            newG = clamp(newG + noise, 0, 255);
            newR = clamp(newR + noise, 0, 255);

            //Zapisanie przerobionych warto�ci RGB do bufora
            pixelBuffer[pixelIndex] = static_cast<unsigned char>(newB);     //kana� Blue
            pixelBuffer[pixelIndex + 1] = static_cast<unsigned char>(newG); //kana� Green
            pixelBuffer[pixelIndex + 2] = static_cast<unsigned char>(newR); //kana� Red
        }
    }
}
