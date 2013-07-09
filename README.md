CodeRefractor
=============

CIL  to Native C++ code

How to test it?

Start: CodeRefractor.sln in your preferred C# IDE (SharpDevelop, XamarinStudio, Visual Studio Express)

Edit the code inside SimpleAdditions project.

Run the solution. As a result, inside bin folder, if everything goes nicely, there will be an SimpleAdditions_cr.exe which reflects your original code.

Possible issues:
- compilation issues: look into GccOptions class and set the options to your locations to GCC distribution. We test using Orwell DevC++. Tweak it to match your locations
- resulting code is incorrect: Report as an issue in the issues section inside original's GitHub project:
 https://github.com/ciplogic/CodeRefractor
- code is too slow: did you enable optimizations? Is it slower than C#? If it is, look if you use a lot of allocations, C++ code has a slower allocator than .Net in most cases
- other issues: generics, delegates, etc. do not work. We know, please try to add the simplest of them, or support the project by donating

Look here for what is freshly developing, and if you think that you want to sped up a feature, don't be shy!
http://coderefractor.blogspot.com/

