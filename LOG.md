## DAN 1

Dodala sam pod, zid i igraca. Napisana skripta za hodanje igraca (PlayerController.cs). Kada su pod i zid preklopljeni, igrac ode na zid zbog toga (provjeriti kako to zaobici, ja sam rn maknula da se nmg preklopiti). Prilagoditi **moveSpeed** i odabrati fje **Lerp** ili **MoveTowards** u PlayerController.cs. Gizmo (zuti krug) se vidi samo u Scene View. Dodan i callback tako da se nova pozicija kopira, obrise i pozove, nema lancanih pokreta i beskonacne petlje. 
MoveTo prima callback jer PlayerController nikad ne mora znati sto postoji u restoranu.

## DAN 2

Dodala Table.cs i TableState.cs. Prazni GameObjecti se koriste kao markeri pozicije. Jednom kada zamjenim sa spriteom, smanjit cu si muke. Koristit cu Observer pattern kasnije. I imam dodan TableManager.cs koji je Singleton da imam jednu instancu jer je opseg mali. 
Napravljen mali tester za clicker tj. 3 testa. Prvi u kojem desni klik na stol vrti stanja automata u krug i mijenja boje. Drugi u kojem lijevi klik na stol postavi igraca na servePos te zadnji u kojem lijevi klik na pod pokrece playera. 
Mijesam konstruktor i Awake jer su slicno predstavljeni, ali Unity zove konstruktor, a Awake je samo priprema. 
Imala sam veliki problem s trecim stolom koji mi nije htio nista raditi zbog collisiona izmedu stola (layer Furniture) i poda (layer Floor). Rijeseno je na nacin da je uveden layer **Interactable** pa prije provjere poda pita ima li tog objekta, ako ima ne dira cilj kretanja.

**DODATI VISE GOSTIJU PO STOLU TJ. AKO STIGNEM, ali da grupa narucuje zajedno**

## DAN 3

Izvukla sam kretanje u posebnu klasu Mover.cs jer sada i gost se treba kretati kao i sto se krece player. Dodan Customer, CustomerState, CustomerSpawner. Customer sam sjedne za stol i ode kada je Dirty i za 4 sekunde se pojavi novi. NOVIH CUSTOMERA NEMA AKO NEMA NITI JEDAN SLOBODAN STOL. Nije dodan RED CEKANJA. 

**TODO: AKO SE STIGNE KASNIJE NAPRAVITI RED CEKANJA**

## DAN 4

Dodan DishSO i MenuSO, a to su ScriptableObjects. Oni nisu objekti nego asseti. Npr. vise gostiju hoce cevape i onda oni dijele istu referencu tj. nema duplikata.  
Dodan i Order koja je obicna C# klasa, samo sluzi za pohranu.
Dodan TableInteractor tj. zamjenio je TableDebugClicker, sada klik na stol radi razlicite stvari. 
TOCTOU - stanje se provjerava prije kretanja i po dolasku. 
FindCustomerAt koristi FindObjectsByType, ali je to lose jer pretrazuje cijelu scenu. Bolje dodati referencu na Customer za svaki Table.
Dodan jednostavan UI, ali se ne brisu prosle narudzbe.

## DAN 5

Dodan treci automat za StoveState i Stove. Dodan CarryController zbog kojeg igrac nosi SAMO JEDNU STVAR, kasnije ubacim upgrade da moze imati vise. TableInteractor je upgradean tj. ima dodane nove funkcije, time malo gubi smisao naziva klase jer se ne brine samo o stolu nego sada i o stednjaku. 

**Jedina stvar koju bi mogla popraviti je da igrac moze ocistiti Dirty stol dok nosi narudzbu.**

## DAN 6

Koristena je korutina za gosta dok jede. To je metoda koja se moze pauzirati i nastaviti. yield return null -> nastavi u sljedecem frame-u. Dodan Money UI, gdje odsada kada player dode na stol, odmah ga OCISTI i COLLECTA payment. Morale su se updejtat fje u Table-u, Customer-u i TableInteractor-u. I odsada Table ima referencu na trenutnog Customera. Table je vlasnik veze,a Customeri mu se prijavljuju i odjavljuju. Dish-ovi sada imaju cijene. OnMoneyChanged nosi iznos (NE STANJE).

**PROMIJENITI CIJENE JELA**

## DAN 7

Dodan DayManager, znaci sada postoji kvota za svaki dan koja se mora ispuniti, ako se ne ispuni, igra se ponovno taj dan, ako se ispuni, ide se na sljedeci dan. Nakon svakog dana postoji reset scene. Time.timeScale = 0 -> zamrzava cijelu igru, na 1 ga vraca. Dodan HUD za dan i timer.

## DAN 8

Dodano strpljenje za goste, tako da mogu otici ako ih se ne posluzi, player nece biti placen ako vec napravi jelo za njih, tako da tu postoji dodatan rizik nezaradenog novca, a ulozenog vremena. Timer za strpljenje se sastoji od tri faze: zelene, zute i crvene. Igrac sada moze ispustiti jelo s desnim klikom. 

**KASNIJE SREDITI BRZINU CEKANJA ITD**