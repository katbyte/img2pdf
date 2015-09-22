//Copyright © 2014 kt@katbyte.me
using System.Reflection;
using System.Runtime.InteropServices;


//assembly GUID
[assembly: Guid("6B617420-0770-1987-0770-627974650001")]


//static assembly properties:
[assembly: AssemblyTitle        ("img2pdf")]
[assembly: AssemblyDescription  ("Simple utility to create pdfs from images.")]
[assembly: AssemblyProduct      ("img2pdf")]

[assembly: AssemblyCompany      ("katbyte.me")]

[assembly: AssemblyCopyright    ("Copyright © 2014 kt@katbyte.me")]
[assembly: AssemblyTrademark    ("Mark of the Bewilderebeest")]

[assembly: AssemblyCulture      ("")]


//com visibility
[assembly: ComVisible(false)]


// dynamically set AssemblyConfiguration property to 3.5|4.5 debug|release
[assembly: AssemblyConfiguration(
    #if DEBUG
        #if DOTNET_35
            "3.5.debug"
        #else
            "4.5.debug"
        #endif
    #else
        #if DOTNET_35
            "3.5.release"
        #else
            "4.5.release"
        #endif
    #endif
)]