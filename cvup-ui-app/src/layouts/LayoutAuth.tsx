import { Button } from "@mui/material";
import { Outlet } from "react-router-dom";
import { useStore } from "../Hooks/useStore";
import { Link, useNavigate } from "react-router-dom";

export const LayoutAuth: React.FC = () => {
  const navigate = useNavigate();
  const rootStore = useStore();
  const { authStore } = rootStore;

  return (
    <div>
      <Button
        fullWidth
        size="large"
        type="submit"
        variant="contained"
        color="secondary"
        onClick={(e) => {
          authStore.logout();
          navigate("/login");
        }}
      >
        Logout
      </Button>
      <div>Layout Auth</div>
      <Outlet />
    </div>
  );
};
