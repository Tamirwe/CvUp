import { Worker } from "@react-pdf-viewer/core";
import Router from "./Router";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";
import "./App.css";
import "./App.scss";
import { useEffect, useState } from "react";
import { IAppSettings } from "./models/GeneralModels";
import { createTheme, ThemeProvider } from "@mui/material";
import { CacheProvider } from "@emotion/react";
import createCache from "@emotion/cache";
import rtlPlugin from "stylis-plugin-rtl";

// import { ThemeCustomization } from "./themes/ThemeCustomization";

function App() {
  const [isServersLoaded, setIsServersLoaded] = useState(false);
  const [appSettings, setAppSettings] = useState<IAppSettings>({
    appServerUrl: "",
  });

  useEffect(() => {
    fetch("./servers.json")
      .then((res) => res.json())
      .then((data) => {
        // appSettings.appServerUrl = data.appHttp;
        setIsServersLoaded(true);
        setAppSettings(data);
        Object.freeze(appSettings);
      });
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // const rootStore = new RootStore();

  const getRootStore = () => {
    return new RootStore(appSettings);
  };

  const themeRtl = createTheme({
    direction: "rtl", // Both here and <body dir="rtl">
  });

  // Create rtl cache
  const cacheRtl = createCache({
    key: "muirtl",
    stylisPlugins: [rtlPlugin],
  });

  return (
    // <ThemeCustomization>
    <>
      {isServersLoaded ? (
        <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
          <CacheProvider value={cacheRtl}>
            <ThemeProvider theme={themeRtl}>
              <StoreProvider store={getRootStore()}>
                <Router />
              </StoreProvider>
            </ThemeProvider>
          </CacheProvider>
        </Worker>
      ) : (
        <div></div>
      )}
    </>
    // </ThemeCustomization>
  );
}

export default App;
