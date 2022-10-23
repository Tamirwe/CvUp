import { Route, Routes } from "react-router-dom";
import { Dashboard } from "./pages/Dashboard";
import { Home } from "./pages/Home";
import { Layout } from "./layouts/Layout";
import { NotFound } from "./pages/NotFound";
import { LayoutAuth } from "./layouts/LayoutAuth";
import { LayoutNotAuth } from "./layouts/LayoutNotAuth";
import { StoreProvider } from "./services/StoreProvider";
import { RootStore } from "./store/RootStore";
import { Login } from "./pages/authentication/Login";
import { Register } from "./pages/authentication/Register";
import { ForgotPassword } from "./pages/authentication/ForgotPassword";
import { Terms } from "./pages/authentication/Terms";
// import { ThemeCustomization } from "./themes/ThemeCustomization";

function App() {
  const rootStore = new RootStore();

  return (
    // <ThemeCustomization>
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
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/terms" element={<Terms />} />
          </Route>
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </StoreProvider>
    // </ThemeCustomization>
  );
}

export default App;
