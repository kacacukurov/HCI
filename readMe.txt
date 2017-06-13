Da bi se program pokrenuo, mora se podesiti sledece platforma, odnosno, pratite ove korake:
 - desni klik na resenje projekta (Solution)
 - kliknite na Configuration Manager
 - u meniju Acitve solution platform treba da odaberete x86 (ukoliko ne postoji, dodajte je)
 - u tabeli kod trenutnog projekta, postavite Platform na x85 (ukoliko ne postoji, dodajte ga)

Ono sto je potrebno da imate instalirano u NuGet Package Manager-u je:
 - cef.redist.x64
 - cef.redist.x86
 - cefSharp.Common
 - cefSharp.Wpf
 - extended.wpf.toolkit
 - thinksharp.featureTour
 - toastNotifications
 - toastNotifications.Messages
 - toggleSwitch
 - wpfToolkit

Program se pokrece u Debug mode-u, jer se u tom folderu nalazi fajl sa predefinisanim podacima.