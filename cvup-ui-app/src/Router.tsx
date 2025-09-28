import { Navigate, Outlet, Route, Routes } from "react-router-dom";
import { Dashboard } from "./pages/Dashboard";
import { NotFound } from "./pages/NotFound";
import { LayoutAuthWrapper } from "./layouts/LayoutAuthWrapper";
import { LayoutNotAuth } from "./layouts/LayoutNotAuth";
import { Login } from "./pages/authentication/Login";
import { Register } from "./pages/authentication/Register";
import { ForgotPassword } from "./pages/authentication/ForgotPassword";
import { Terms } from "./components/authentication/Terms";
import { PasswordReset } from "./pages/authentication/PasswordReset";
import { useStore } from "./Hooks/useStore";
import { CompleteRegistration } from "./pages/authentication/CompleteRegistration";
import { Position } from "./pages/Position";
import { CvPage } from "./pages/CvPage";
import { CandidatesReport } from "./pages/CandidatesReport";
import { FuturesStatistics } from "./pages/FuturesStatistics";

const AuthRoutes = () => {
  const rootStore = useStore();
  const { authStore } = rootStore;

  return authStore.isLoggedIn ? <Outlet /> : <Navigate to="/login" />;
};

const Router = () => {
  return (
    <Routes>
      <Route element={<AuthRoutes />}>
        <Route element={<LayoutAuthWrapper />}>
          <Route path="/" element={<Dashboard />} />
          <Route path="/cv" element={<CvPage />} />
          <Route path="/position/:pid" element={<Position />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/candidatesreport" element={<CandidatesReport />} />
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
        <Route path="/fustat" element={<FuturesStatistics />} />
      </Route>
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default Router;
