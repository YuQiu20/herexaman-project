Beste lezer

Dit is een project dat ik heb gemaakt voor mijn herkansing.
Ik heb als opdracht een oude RS485 converter ontvangen dat ik moet vernieuwen en een programma voor schrijven.
ik moet gebruik maken van een FT230XS-R chip waarvan dat er een voorbeeldschema aanwezig in de datasheet dat hielp voor het hertekenen van mijn schema.
In plaats van USB B moet ik een USB C gebruiken wat ook handiger is voor nieuwere computers voor de RS485 transceiver maak ik gebruik van de ADM3485EARZ omdat de tranceiver in de datasheet de ZT3485 niet meer beschikbaar is.
op de cc-pinnen van de USB C voorzie ik een weerstand van 10K dit zorgt ervoor dat de computer het bordje herkent als een apparaart zonder dit zal er geen verzending mogelijk zijn.
voor mijn print voor zie ik 30mm x 70mm dit is ook het standaard dat ik heb gekregen bij de uitleg.
voor 3D ontwerp kwam mijn idee eerst van een scharnier, het is een doosje met een scharnier en een klik-systeem zo dat het makkelijk te gebruiken is.
er zijn in de doos 4 staven voorzien dat het pcb bordje vast houd als je het doosje toe doet en de klik-systeem zorgt er voor dat het doosje niet van zelf open gaat.
als programma heb ik verder opgebouwd op de programma dat aanwezig op toledo van meneer Buysshaert, hiermee zorg ik ervoor dat ik de dmx kan aansturen via mijn computer, er zijn 2 knoppen aanwezig voor alle les te laten braden en alles uit laten gaan.
er zijn ook sliders aanwezig dat voor de kleuren rood groen en blauw en ten laatste is er ook een timer dat de dmx groen kleurt en dan rood een zogezegde start stop.

start up------------------------
als de programma start dan kan je de com-poort selectern en als je de verkeerde com-poort selecteer zal het een melding geven dat het niet kan verbinden

RGB sliders-------------------------
met de slider in de programma kan je elke led een kleur geven zoals rood groen blauw

3 knoppen--------------------------
er zijn 3 geknoppen geprogrameerd 1 voor alle led aan te laten branden op wit
1 voor alles uit dus alles op 0 zetten
en er is een 3e knop voor kleurcyclus voor groen naar rood en omgekeerd met een tijd dat je kan invullen in ms
alle slider bewegen dan mee als we door de kleurcyclus gaan.

componentn--------------
FT230XS-R
ADM3585EARZ
XLR socket
weerstanden SMD van 10k, 27Ohm en 120Ohm
condensatoren SMD van 47pF, 4.7pF 10nF en 100nF
pin contact 1x2
