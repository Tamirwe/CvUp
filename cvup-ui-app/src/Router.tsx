import { lazy, Suspense } from "react";
import { Navigate, Outlet, Route, Routes } from "react-router-dom";
import { NotFound } from "./pages/NotFound";
import { LayoutNotAuth } from "./layouts/LayoutNotAuth";
import { Login } from "./pages/authentication/Login";
import { Register } from "./pages/authentication/Register";
import { ForgotPassword } from "./pages/authentication/ForgotPassword";
import { Terms } from "./components/authentication/Terms";
import { PasswordReset } from "./pages/authentication/PasswordReset";
import { useStore } from "./Hooks/useStore";
import { CompleteRegistration } from "./pages/authentication/CompleteRegistration";

// The authenticated pages pull in the heavy leaves -- the pdf viewer, the rich
// text editor, the charts -- none of which the login screen has any use for.
// Loading them on demand keeps them off the initial page load.
//
// LayoutAuthWrapper matters most: it eagerly imports about twenty dialogs,
// among them InterviewFullDialog (the pdf viewer) and the email senders (the
// quill editor), so lazy pages alone would not have deferred either.
const LayoutAuthWrapper = lazy(() =>
  import("./layouts/LayoutAuthWrapper").then((m) => ({
    default: m.LayoutAuthWrapper,
  }))
);
const Dashboard = lazy(() =>
  import("./pages/Dashboard").then((m) => ({ default: m.Dashboard }))
);
const CvPage = lazy(() =>
  import("./pages/CvPage").then((m) => ({ default: m.CvPage }))
);
const Position = lazy(() =>
  import("./pages/Position").then((m) => ({ default: m.Position }))
);
const CandidatesReport = lazy(() =>
  import("./pages/CandidatesReport").then((m) => ({
    default: m.CandidatesReport,
  }))
);
const FuturesStatistics = lazy(() =>
  import("./pages/FuturesStatistics").then((m) => ({
    default: m.FuturesStatistics,
  }))
);

const AuthRoutes = () => {
  const rootStore = useStore();
  const { authStore } = rootStore;

  return authStore.isLoggedIn ? <Outlet /> : <Navigate to="/login" />;
};

const Router = () => {
  return (
    <Suspense fallback={<div></div>}>
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
    </Suspense>
  );
};

export default Router;
