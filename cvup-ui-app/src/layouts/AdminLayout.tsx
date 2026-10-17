import { Box } from "@mui/material";
import { Outlet, useNavigate } from "react-router-dom";
import { observer } from "mobx-react";
import logo from "../assets/images/CvUpLogo2.png";

// Deliberately undesigned: the admin dashboard itself is still to be built.
// The only thing wired up here is the logo, which is the way back to the main
// (recruiter) layout -- the same gesture that got the user here.
export const AdminLayout = observer(() => {
  const navigate = useNavigate();

  return (
    <Box sx={{ flexGrow: 1 }}>
      <div
        style={{
          height: "2.78rem",
          padding: "1.0rem 0rem 0rem 1rem",
          position: "relative",
        }}
      >
        <img
          src={logo}
          alt="CvUp"
          title="Back to the main app"
          onClick={() => navigate("/")}
          style={{
            width: "5rem",
            position: "absolute",
            top: "5px",
            cursor: "pointer",
          }}
        />
      </div>
      <Outlet />
    </Box>
  );
});
