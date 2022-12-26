import { Worker } from "@react-pdf-viewer/core";
import Router from "./Router";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";
import "./App.css";
import "./App.scss";

// import { ThemeCustomization } from "./themes/ThemeCustomization";

function App() {
  const rootStore = new RootStore();

  return (
    // <ThemeCustomization>
    <Worker workerUrl="https://unpkg.com/pdfjs-dist@3.1.81/build/pdf.worker.min.js">
      <StoreProvider store={rootStore}>
        <Router />
      </StoreProvider>
    </Worker>
    // </ThemeCustomization>
  );
}

export default App;
