# Sepia-and-Noise-Effect-Processor-C#-Windows-Forms-GUI-with-CPP-and-Assembly-DLLs
This project is an image processing application that applies a sepia filter with a noise effect to an image. The software features a Windows Forms GUI (C#) that dynamically loads two processing libraries:

C++ DLL – A high-level procedural implementation.
Assembly (x86-64) DLL – Optimized using SIMD (Single Instruction, Multiple Data) instructions for high-performance parallel processing.
The user can choose between C++ and ASM implementations to compare performance while maintaining the same visual output.

Features
✔ Windows Forms GUI (C#) for user-friendly interaction.
✔ Applies a sepia filter to create an aged photo effect.
✔ Adds a noise effect to introduce randomness in pixel values.
✔ Two dynamically loaded DLLs:

C++ DLL – Standard procedural image processing.
ASM DLL – Optimized with AVX/SSE instructions for high-speed calculations.
✔ Multithreading support – Users can select 1–64 threads for better performance.
✔ Execution time tracking – Compare C++ vs ASM processing speeds.

🛠 Technologies Used
C# ( Windows Forms) – GUI and DLL integration.
C++ (Visual Studio 2022, DLL) – High-level image processing.
x86-64 Assembly (MASM, DLL) – Optimized low-level implementation.
SIMD (AVX/SSE) – Accelerated pixel processing.
Multithreading – Parallel execution for speed improvement.

📌 Important Configuration Step
To run this project on your computer, you need to update the paths to the DLL files in Form1.cs.

By default, the DLLs are loaded using:
[DllImport(@"C:\Users\Julia\Desktop\Sepia\x64\Release\DLLCPP.dll")]
[DllImport(@"C:\Users\Julia\Desktop\Sepia\x64\Release\DLLASM.dll")]
Solution: Modify these paths to match the actual location of DLLCPP.dll and DLLASM.dll on your computer.
