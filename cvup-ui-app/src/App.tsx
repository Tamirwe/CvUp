import Router from "./Router";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";

// import { ThemeCustomization } from "./themes/ThemeCustomization";

function App() {
  const rootStore = new RootStore();

  return (
    // <ThemeCustomization>
    <StoreProvider store={rootStore}>
      <Router />
    </StoreProvider>
    // </ThemeCustomization>
  );
}

export default App;
