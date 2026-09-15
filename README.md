# Segui la foll(i)a /Effetto Mandria/

Applicazione web del gioco da tavolo **Effetto Mandria**: domande a risposta aperta o multipla, votazione collettiva e punteggio in stile “mandria”.

# AI slop warning
Parts of this project (including docs, copy, and some implementation drafts) is AI-assisted. Treat generated content as a starting point, not as guaranteed truth. While I checked what AI tools were doing, NEVER TRUST AI. 
Well, this application is client only with a simple backend but no database, no personal data etcs... So, no actual danger, but still... be careful.

NEVER TRUST GENERATIVE AI BLINDLY. Always review, test, and validate behavior before using it in production.



## Stack

- **Frontend:** Vue 3, TypeScript, Pinia, Vue Router, SignalR client
- **Backend:** ASP.NET Core (.NET 10), API REST + SignalR, tutto **in memoria** (nessun database)

## Avvio rapido (sviluppo)

### Backend

```bash
cd backend/EffettoMandria.Api
dotnet run
```

API e hub SignalR su `http://localhost:5080` (anche `http://0.0.0.0:5080` per accesso da telefono sulla LAN).

### Frontend

```bash
cd frontend
npm install
npm run dev
```

UI su `http://localhost:5173` (in ascolto su `0.0.0.0` per accesso da LAN, es. `http://<IP-PC>:5173`) con proxy verso l’API.

### Dev: reverse proxy (consigliato, niente CORS)

Con `npm run dev`, lascia **`VITE_API_BASE_URL` vuoto** in `.env.development`. Vite inoltra `/api` e `/hubs` al backend (`VITE_DEV_PROXY_TARGET`, default `https://localhost:7074`).

Il telefono apre `http://<IP-PC>:5173` → le chiamate restano same-origin verso Vite, che fa da proxy verso l’API sul PC.

Se avvii il backend solo in HTTP (`http://localhost:5230`), imposta:

```env
VITE_DEV_PROXY_TARGET=http://localhost:5230
```

### Produzione

| Scenario | `VITE_API_BASE_URL` |
|----------|---------------------|
| UI servita dal backend (`wwwroot`) | vuoto |
| UI e API su host diversi | URL completo dell’API |

CORS sul backend serve solo per accesso diretto all’API senza proxy (`Cors:AllowedOrigins` in `appsettings.Development.json`).

Opzionale: `VITE_HIDE_AUTH_BUTTONS=true` nasconde **Accedi** / **Registrati** nella home (e il link account in `/entra/:id`).

## Ruoli

| Ruolo | URL | Descrizione |
|--------|-----|-------------|
| **Direttore** | `/direttore` → `/direttore/{id}` | PC: classifica, domanda, chi ha risposto/votato, QR per i giocatori |
| **Giocatore** | QR → `/entra/{id}` → `/partita/{id}` | Telefono: risponde e vota |

## Regole implementate

1. **Registrazione** o **ospite** (nome visualizzato).
2. Più **partite** contemporanee; un utente in **una sola partita** alla volta.
3. Turno: fase risposte → votazione (automatica quando tutti hanno risposto) → risultati.
4. **Punteggio:** +1 a chi ha votato la risposta più votata.
5. **Penalità:** se esiste una sola risposta con un solo voto, l’autore riceve una penalità.

Domande caricate da `questions.json` (aperta / multipla con `opzioni`).

## Build e deploy

Da root del repo (PowerShell):

```powershell
.\build.ps1
```

Lo script:

1. Compila il **frontend** con `VITE_API_BASE_URL` vuoto (stesso schema del **reverse proxy Vite** in dev: tutto su un host, `/api` e `/hubs`).
2. Esegue **`dotnet publish`** del backend in `./publish`.
3. Copia `frontend/dist` in `publish/wwwroot`.

Avvio della build pubblicata (es. party su LAN):

```powershell
cd publish
dotnet EffettoMandria.Api.dll
```

Environment e URL sono in `backend/EffettoMandria.Api/appsettings.Production.json` (`Hosting:Environment`, `Hosting:Urls`, `Kestrel`). In publish viene copiato accanto al DLL; non servono variabili d'ambiente (salvo override).

Apri `http://<IP-del-server>:5080` da PC e telefoni.

Opzioni: `.\build.ps1 -OutputDir .\out` · `.\build.ps1 -SkipNpmInstall`

### Release GitHub e immagine Docker

Il workflow `.github/workflows/release.yml` viene eseguito quando viene pubblicato un tag semantico `v*`:

```bash
git tag v1.0.0
git push origin v1.0.0
```

Il workflow crea una GitHub Release con l'archivio dell'applicazione e pubblica l'immagine su GitHub Container Registry:

```text
ghcr.io/andreaci/seguilafolla:1.0.0
ghcr.io/andreaci/seguilafolla:latest
```

Esecuzione del container:

```bash
docker run --rm -p 5080:5080 ghcr.io/andreaci/seguilafolla:latest
```

Può anche essere avviato manualmente dalla pagina **Actions**, indicando il tag della release. Le prerelease, ad esempio `v1.0.0-beta.1`, non aggiornano il tag Docker `latest`.

## Struttura

```
muccamento/
  questions.json          # banca domande
  backend/EffettoMandria.Api/
  frontend/src/
    api/                  # client HTTP + SignalR
    stores/               # auth, game
    components/           # UI riutilizzabili
    views/                # schermate
    styles/               # CSS condiviso
```
