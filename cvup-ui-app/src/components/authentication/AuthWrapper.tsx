import { Grid } from "@mui/material";

interface props {
  children: React.ReactNode;
}

export const AuthWrapper = ({ children }: props) => {
  return (
    <Grid container direction="column" sx={{ minHeight: "100vh" }}>
      <Grid item xs={12}>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          sx={{ minHeight: "calc(100vh - 68px)" }}
        >
          {children}
        </Grid>
      </Grid>
    </Grid>
  );
};
