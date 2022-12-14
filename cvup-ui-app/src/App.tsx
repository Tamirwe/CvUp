import { Worker } from "@react-pdf-viewer/core";
import Router from "./Router";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";
import "./App.css";
import "./App.scss";
import { useEffect, useState } from "react";
import { IAppSettings } from "./models/GeneralModels";

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

  return (
    // <ThemeCustomization>
    <>
      {isServersLoaded ? (
        <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
          <StoreProvider store={getRootStore()}>
            <Router />
          </StoreProvider>
        </Worker>
      ) : (
        <div></div>
      )}
    </>
    // </ThemeCustomization>
  );
}

export default App;
