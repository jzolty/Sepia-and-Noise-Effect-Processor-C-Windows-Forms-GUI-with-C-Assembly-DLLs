#include "pch.h"
#include <iostream>
#include <cstdlib>   // rand()    
#include <windows.h>

//Funkcja pomocnicza do ograniczania wartoœci do zakresu [low, high]
//Jeœli wartoœæ jest mniejsza ni¿ 'low', zwraca 'low', jeœli wiêksza ni¿ 'high', zwraca 'high'.
//W przeciwnym razie zwraca oryginaln¹ wartoœæ.
inline int clamp(int value, int low, int high) {
    return (value < low) ? low : (value > high ? high : value);
}

//Eksportowana funkcja ApplySepiaFilter, która przyjmuje zakres wierszy obrazu do przetworzenia
extern "C" __declspec(dllexport) void ApplySepiaFilter(unsigned char* pixelBuffer, int width, int bytesPerPixel, int P, int X, int startRow, int endRow, int stride)
{
    // Iteracja po wierszach obrazu od startRow do endRow
    for (int y = startRow; y < endRow; y++)
    {
        //Pocz¹tek bie¿¹cego wiersza w buforze pamieci
        int rowStart = y * stride;

        // Iteracja po pikselach w bie¿¹cym wierszu
        for (int x = 0; x < width; x++)
        {
            // Obliczenie indeksu bie¿¹cego piksela w buforze
            int pixelIndex = rowStart + x * bytesPerPixel;

            //Pobranie kana³ów RGB z bufora
            unsigned char B = pixelBuffer[pixelIndex];     //Blue
            unsigned char G = pixelBuffer[pixelIndex + 1]; //Green
            unsigned char R = pixelBuffer[pixelIndex + 2]; //Red

            //Konwersja na odcieñ szaroœci
            int gray = static_cast<int>(0.299 * R + 0.587 * G + 0.114 * B);

            // Wyliczenie nowych wartoœci kana³ów dla efektu sepii
            int newB = gray;
            int newG = P + gray;
            int newR = 2 * P + gray;

            // Ograniczenie wartoœci do zakresu [0, 255]
            newB = clamp(newB, 0, 255);
            newG = clamp(newG, 0, 255);
            newR = clamp(newR, 0, 255);

            // Dodanie efektu postarzenia z parametrem X
            int noise = (rand() % (2 * X + 1)) - X; // losowa wartosc z przedzia³u [-X, X]
            newB = clamp(newB + noise, 0, 255);
            newG = clamp(newG + noise, 0, 255);
            newR = clamp(newR + noise, 0, 255);

            //Zapisanie przerobionych wartoœci RGB do bufora
            pixelBuffer[pixelIndex] = static_cast<unsigned char>(newB);     //kana³ Blue
            pixelBuffer[pixelIndex + 1] = static_cast<unsigned char>(newG); //kana³ Green
            pixelBuffer[pixelIndex + 2] = static_cast<unsigned char>(newR); //kana³ Red
        }
    }
}
