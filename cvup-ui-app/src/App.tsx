import { Route, Routes } from "react-router-dom";
import "./App.css";
import { Dashboard } from "./pages/Dashboard";
import { Home } from "./pages/Home";
import { Layout } from "./layouts/Layout";
import { Login } from "./pages/Login";
import { NotFound } from "./pages/NotFound";
import { Register } from "./pages/Register";
import { LayoutAuth } from "./layouts/LayoutAuth";
import { LayoutNotAuth } from "./layouts/LayoutNotAuth";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";

function App() {
  const rootStore = new RootStore();

  return (
    <>
      <StoreProvider store={rootStore}>
        <Routes>
          <Route element={<Layout />}>
            <Route element={<LayoutAuth />}>
              <Route path="/" element={<Home />} />
              <Route path="/dashboard" element={<Dashboard />} />
            </Route>
            <Route element={<LayoutNotAuth />}>
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
            </Route>
          </Route>
          <Route path="*" element={<NotFound />} />
        </Routes>
      </StoreProvider>
    </>
  );
}

export default App;
