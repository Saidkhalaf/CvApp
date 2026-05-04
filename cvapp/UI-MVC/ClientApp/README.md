# ClientApp (Vite Assets for MVC)

`UI-MVC/ClientApp` is used as an asset pipeline. Razor remains the HTML/layout source.

## Install

```powershell
cd C:\Users\saidk\Stage\stage-cv-app\cvapp\UI-MVC\ClientApp
npm install
```

## Build assets for MVC

```powershell
cd C:\Users\saidk\Stage\stage-cv-app\cvapp\UI-MVC\ClientApp
npm run build
```

This generates bundles into `UI-MVC/wwwroot/dist`:

- `main.css`
- `main.entry.js`

## TypeScript structure convention

For team consistency, each new TypeScript entry should be placed in its own meaningful folder under `src/ts`.

Pattern:

- `src/ts/<feature-name>/<feature-name>.ts`

Example:

- `src/ts/career-form/career-form.ts`

After creating a new entry file, add it to `rollupOptions.input` in `ClientApp/vite.config.js`.

Example:

```javascript
input: {
  main: resolve(__dirname, "src/ts/main.ts"),
  careerForm: resolve(__dirname, "src/ts/career-form/career-form.ts"),
}
```

If an entry is not added to `vite.config.js`, Vite will not build it into `wwwroot/dist`.

## Run MVC app

```powershell
cd C:\Users\saidk\Stage\stage-cv-app\cvapp
dotnet run --project .\UI-MVC\UI-MVC.csproj
```

