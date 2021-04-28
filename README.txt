IDE: Visual Studio 2019 (Konfigurace řešení: Debug, Platforma řešení: Any CPU)
OS: Windows 10


Návod na spuštění:
1. Nejprve je potřeba zkompilovat knihovnu pro agregaci dat
	1.1. Solution této knihovny se nachází na  Agregace Dat/AgregaceDatLib/AgregaceDatLib.sln
	1.2. Otevřeme solution této knihovny ve Visual Studiu a sestavíme ji
2. Následně je potřeba zkompilovat a spustit webovou službu
	2.1. Solution tohoto projektu se nachází na  Webova Sluzba/Webova Sluzba.sln
	2.2. Otevřeme solution ve Visual Studiu a spustíme ji čímž se spustí webový server a vytvoří se veškeré potřebné složky s daty (složka Webova Sluzba/Data) *při mazání bin složky ve Vizualizaci dat zde musí zůstat podsložky x32 a x64 s dll soubory SQLite.Interop.dll
3. Nakonec je potřeba zprovoznit Vizualizační aplikaci
	3.1. Solution aplikace se nachází na Vizualizace Dat/Vizualizace Dat.sln
	3.2. Otevřeme solution ve Visual Studiu a zkompilujeme ji
	3.3. Následně ji spustíme přímo ve Visual Studiu nebo pomocí exe souboru ve Vizualizace Dat/Vizualizace Dat/bin/Debug/Vizualizace Dat.exe
	3.4. Pokud adresa serveru neodpovídá tak nastavíme adresu webového serveru přímo v aplikaci kliknutím na menu -> nastavení -> adresa serveru -> nastavit vlastní adresu

Návod na používání:
1. Webová Služba
	1.1. Zpracovávání bitmap
		1.1.1. Při prvním spuštění služby je potřeba aby veškeré používané datové zdroje zpracovaly a vytvořily své bitmapy, tento proces zabere zhruba 10 minut
		1.1.2. Během vytváření bitmap lze službu používat, je ale možné že požadované bitmapy nebudou ještě zpracovány a bude potřeba počkat na dokončení zpracovávání
		1.1.3. Požadavek na vytváření nových bitmap včetně mazání starých probíhá periodicky každou hodinu, to zda dojde k vytvoření bitmap závisí na posledním zpracování, každý loader má nastavenou potřebnou časovou hranici která musí být překročena aby mohlo dojít k vytváření nových bitmap
		1.1.4. Během zpracovávání bitmap je v debug okně zobrazeno v jakém stavu se zpracovávání nachází, které loadery práci dokončily a zda došlo k nějaké chybě (Yr.no: Zahájení vytváření bitmap, AgregaceDat: Dokončení vytváření bitmap)
	1.2. Poskytování dat
		1.2.1. Služba poskytuje data o počasí prostřednictvím bitmap, xml a json souborů
		1.2.2. Data jsou poskytování prostřednictvím API adresaserveru/bmp, adresaserveru/xml a adresaserveru/json
		1.2.3. Následně je možné získat používané škály adresaserveru/scale a rozmezí maximální zpracovávané plochy bitmapami adresaserveru/bounds
		1.2.4. Příklady používání API s vyplněnými parametry se nachází na stránce adresaserveru/ (adresaserveru/home/index)
		1.2.5. Detailní popis všech možných parametrů a způsobů použití se nachází na stránce adresaserveru/home/details
	1.3. Chybové hlášky
		1.3.1. Pokud nastane při poskytování dat problém, nejčastěji při chybném zadání parametrů pro API dojde k vrácení chybové stránky
		1.3.2. Chybová stránka obsahuje json data s parametrem message obsahujícím informace o tom proč k chybě došlo ({"message": "Žádný ze zvolených datových zdrojů neobsahuje bitmapu se zvoleným časem a typem předpovědi! Zkuste změnit čas případně zadaný typ."})
2. Vizualizační Aplikace
	2.1. Volba prvku počasí
		2.1.1. Prvek počasí se vybírá v levé horní části mezi radio buttony prvků
		2.1.2. Po kliknutí na prvek dochází k vykreslení počasí pro tento prvek
		2.1.3. Zároveň se zobrazí škála používaná pro tento prvek
	2.2. Volba datových zdrojů
		2.2.1. Datové zdroje se volí v pravém horním rohu vybráním požadovaných combo boxů
		2.2.2. Při změně datového zdroje dochází k vykreslení počasí pro nově zvolený datový zdroj
		2.2.3. Zaškrtnuté zdroje které nejsou zešedlé se použijí při požadavku o poskytování dat
	2.3. Počasí v bodě
		2.3.1. Dvojitým kliknutím do mapy dochází k vykreslení počasí v bodě
		2.3.2. Stejně tak máme možnost napsat název místa do textového pole v levé horní částí a potvrdíme kliknutím na vyhledat nebo klávesou enter
		2.3.3. Při vykreslení dojde k zobrazení markeru místa s popisem počasí v tomto bodě
		2.3.4. Zároveň dochází k vykreslení grafu s hodnotou zvoleného prvku počasí pro následující hodiny
		2.3.5. Opětovným kliknutím na marker jej vymažeme případně dvojklikem/napsáním textu vyhledáme počasí pro nový bod a ten starý odstraníme
	2.4. Počasí na trase
		2.4.1. Menu -> Akce -> Nahrát trasu z GPX souboru
		2.4.2. Po vyplnění potřebných informací a zadání GPX souboru dochází k vykreslení počasí na trase
		2.4.3. Ukázkové GPX soubory s trasami trasa1.gpx, trasa2.gpx se nachází ve složce Vizualizace Dat/ (vlastní GPX trasu si můžete vytvořit na https://mapy.cz/zakladni?planovani-trasy)
		2.4.4. Po vykreslení trasy je možné trasu vymazat nebo zadat nový čas zobrazenými tlačítky uprostřed nahoře na mapě
		2.4.5. Při změně datových zdrojů nebo prvku počasí dochází k vykreslení nového počasí pro stejnou trasu
	2.5. Počasí na ploše
		2.5.1. Na mapě se vykresluje bitmapa s daty o počasí pro zvolený prvek a čas
		2.5.2. Při změně času, prvku a datového zdroje dochází k překreslení této bitmapy na aktuální
	2.6. Animace
		2.6.1. Při kliknutí na zelené tlačítko PLAY v pravé dolní části aplikace dochází ke spuštění animace
		2.6.2. Animace má zvolený počet kroků a nastavitelný časový rozdíl mezi kroky
		2.6.3. Animace vykresluje bitmapu s daty o počasí na mapu pro aktuální čas animace
		2.6.4. Na časové liště vidíme čas ve kterém se animace aktuálně nachází a počet kroků zbývajících do konce animace
	2.7. Časová lišta
		2.7.1. Ve spodní části aplikace se nachází časová lišta pro nastavení času počasí
		2.7.2. Změnou času na liště dochází k překreslení bitmapy na mapě pro aktuálně zvolený čas
		2.7.3. Pro zvolená čas bude vykresleno počasí v bodě
		2.7.4. Na zvoleném čase bude začínat animace

Návod na nastavení:
1. Agregace Dat
	1.1. Přidání datových zdrojů
		1.1.1. Ve složce Agregace Dat\DatoveZdrojeAHandlery\DataLoadersLibs se nachází veškeré aktuálně používané datové zdroje, respektive jejich dll soubory
		1.1.2. Vložením nového dll souboru dochází k přidání nového datového zdroje, který se načte za běhu pomocí reflexe
		1.1.3. Zdroje mohou využívat pomocné třídy u knihovny IDataLoaderAndHandlerLib, dědit z předpřipraveného zdroje a používat triangulaci + převod barev na číslo a naopak
		1.1.4. Zdroj musí implementovat rozhraní DataLoader a soubor + namespace musí nést název DLNazevLoaderuLib.dll, knihovna musí nést název NazevLoaderuDataLoader
	1.2. Úprava aktuálních datových zdrojů	
		1.2.1. Je možné upravit kód používaných datových zdrojů které se nachází ve složce Agregace Dat\DatoveZdrojeAHandlery\DataLoaders
		1.2.2. Po zkompilování knihovny je potřeba dll soubor zdroje přenést do složky Agregace Dat\DatoveZdrojeAHandlery\DataLoadersLibs
		1.2.3. Případně je možné změnit hodnotu proměnné inDebug v Agregaci Dat -> AvgForecast na true a budou se používat dll soubory z bin adresářů jednotlivých solutions datových zdrojů (pokud se nejprve zkompilují)
	1.3. Změna hranic maximální vytvářené bitmapy
		1.3.1. Při změně hranic v konfiguračním json souboru pro agregaci dat dojde ke změně maximální plochy zpracovávaných bitmap
		1.3.2. Cesta k tomuto souboru Webova Sluzba\Webova Sluzba\Data\agrConfig.json
	1.4. Změna škál
		1.4.1. Máme možnost používat vlastní vytvořené škály, škály mají 1 pixel na výšku a X pixelů na šířku
		1.4.2. Cesta ke škálám Webova Sluzba\Webova Sluzba\Data\ŠKÁLA.png
	1.5. Změna vlastností zdrojů
		1.5.1. Každý ze zdrojů má svůj konfigurační json soubor obsahující data o názvu, zkratce, limitacích na stažení, potřebných časech mezi staženími atp
		1.5.2. Tento soubor se nachází na Webova Sluzba\Webova Sluzba\Data\NÁZEV_ZDROJE\loaderConfig.json a je možné změnit jeho parametry
2. Webová Služba
	2.1. Změna času opakujícího se požadavku na znovu vykreslení bitmap
		2.1.1. Pokud nechceme aby naše webová služba prováděla kontrolu na vytváření nových bitmap co hodinu ale častěji/méně často musíme odkomentovat jeden ze řádků v kódu
		2.1.2. Třída TimedHostedService.cs, Metoda public Task StartAsync(CancellationToken cancellationToken)
3. Vizualizační Aplikace
	3.1. Změna adresy webové služby
		3.1.1. Pokud chceme změnit adresu webového serveru můžeme to udělat přes nastavení nebo přímo v json config souboru
		3.1.2. Přes nastavení přímo v aplikaci, klikneme na menu v horní části -> nastavení -> adresa serveru -> nastavit vlastní adresu
		3.1.3. Přes json soubor, atribut ServerAddress, cesta k souboru Vizualizace Dat\Vizualizace Dat\Resources\aojConfig.json

