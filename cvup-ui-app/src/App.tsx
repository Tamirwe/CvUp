import { Worker } from "@react-pdf-viewer/core";
import Router from "./Router";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";
import "./App.css";
import "./App.scss";
import { useEffect, useState } from "react";
import { IAppSettings, IAppSettingsFile } from "./models/GeneralModels";

import { ThemeCustomization } from "./themes/ThemeCustomization";

function App() {
  const [isServersLoaded, setIsServersLoaded] = useState(false);
  const [appSettings, setAppSettings] = useState<IAppSettings>({
    apiUrl: "",
    appMode: "",
  });

  useEffect(() => {
    const storedSettings = localStorage.getItem("settings");

    if (storedSettings) {
      setAppSettings(JSON.parse(storedSettings));
      setIsServersLoaded(true);
    } else {
      try {
        fetch(`${process.env.PUBLIC_URL}/appSettings.json`)
          .then((res) => res.json())
          .then((data: IAppSettingsFile) => {
            setIsServersLoaded(true);
            const defaultApiUrl = data.servers[0].apiUrl;
            const settingsObj = {
              apiUrl: defaultApiUrl,
              appMode: data.appMode,
            };
            localStorage.setItem("settings", JSON.stringify(settingsObj));
            setAppSettings(settingsObj);
            Object.freeze(appSettings);
          });
      } catch (error) {}
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // const rootStore = new RootStore();

  const getRootStore = () => {
    return new RootStore(appSettings);
  };

  return (
    <ThemeCustomization>
      {isServersLoaded ? (
        <StoreProvider store={getRootStore()}>
          <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
            <Router />
          </Worker>
        </StoreProvider>
      ) : (
        <div></div>
      )}
    </ThemeCustomization>
  );
}

export default App;
