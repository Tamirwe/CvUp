import Router from "./Router";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";
import "./App.css";
import "./App.scss";
import { useState } from "react";

import { ThemeCustomization } from "./themes/ThemeCustomization";

function App() {
  const [rootStore] = useState(() => new RootStore());

  return (
    <ThemeCustomization>
      <StoreProvider store={rootStore}>
        <Router />
      </StoreProvider>
    </ThemeCustomization>
  );
}

export default App;
