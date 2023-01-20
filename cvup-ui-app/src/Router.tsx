import { Navigate, Outlet, Route, Routes } from "react-router-dom";
import { Dashboard } from "./pages/Dashboard";
// import { Home } from "./pages/Home";
import { NotFound } from "./pages/NotFound";
import { LayoutAuth } from "./layouts/LayoutAuth";
import { LayoutNotAuth } from "./layouts/LayoutNotAuth";
import { Login } from "./pages/authentication/Login";
import { Register } from "./pages/authentication/Register";
import { ForgotPassword } from "./pages/authentication/ForgotPassword";
import { Terms } from "./components/authentication/Terms";
import { PasswordReset } from "./pages/authentication/PasswordReset";
import { useStore } from "./Hooks/useStore";
import { CompleteRegistration } from "./pages/authentication/CompleteRegistration";
import { Cv } from "./pages/Cv";
import { Position } from "./pages/Position";

const AuthRoutes = () => {
  const rootStore = useStore();
  const { authStore } = rootStore;

  return authStore.isLoggedIn ? <Outlet /> : <Navigate to="/login" />;
};

const Router = () => {
  return (
    <Routes>
      <Route element={<AuthRoutes />}>
        <Route element={<LayoutAuth />}>
          <Route path="/" element={<Cv />} />
          <Route path="/cv" element={<Cv />} />
          <Route path="/position/:pid" element={<Position />} />
          {/* <Route path="/h1" element={<Home />} /> */}
          <Route path="/dashboard" element={<Dashboard />} />
        </Route>
      </Route>
      <Route element={<LayoutNotAuth />}>
        <Route path="/" element={<Login />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route
          path="/complete-registration"
          element={<CompleteRegistration />}
        />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/password-reset" element={<PasswordReset />} />
        <Route path="/terms" element={<Terms />} />
      </Route>
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default Router;
