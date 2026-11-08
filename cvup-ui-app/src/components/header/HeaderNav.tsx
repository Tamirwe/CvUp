import { Button, Divider, Stack } from "@mui/material";
import { CiLogout } from "react-icons/ci";
import { MdGroup } from "react-icons/md";
import { useNavigate } from "react-router-dom";
import { useStore } from "../../Hooks/useStore";
import { SettingsMenu } from "./SettingsMenu";

const navButtonSx = { textTransform: "none", whiteSpace: "nowrap" };

export const HeaderNav = () => {
  const { generalStore, authStore } = useStore();
  const navigate = useNavigate();

  return (
    <Stack
      component="nav"
      direction="row"
      alignItems="center"
      spacing={0.5}
      divider={<Divider orientation="vertical" flexItem />}
    >
      <SettingsMenu withLabel />
      <Button
        color="inherit"
        startIcon={<MdGroup />}
        sx={navButtonSx}
        onClick={() => (generalStore.showCustomersListDialog = true)}
      >
        Customers
      </Button>
      <Button
        color="inherit"
        startIcon={<CiLogout />}
        sx={navButtonSx}
        onClick={() => {
          authStore.logout();
          navigate("/login");
        }}
      >
        Logout
      </Button>
    </Stack>
  );
};
