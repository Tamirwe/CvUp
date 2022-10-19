import {
  Grid,
  Stack,
  Typography,
  useMediaQuery,
  useTheme,
  Divider,
  responsiveFontSizes,
} from "@mui/material";
import { Link } from "react-router-dom";
import { LoginForm } from "./LoginForm";

interface props {}

export const LoginWrapper = ({}: props) => {
  const theme = useTheme();
  const matchDownSM = useMediaQuery(theme.breakpoints.down("md"));

  return (
    <Grid container>
      <Grid item xs={12}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          spacing={1}
        >
          <Typography
            color={theme.palette.secondary.main}
            gutterBottom
            variant="h5"
          >
            Login
          </Typography>
          <Typography component={Link} to="/register" variant="subtitle2">
            Don&apos;t have an account?
          </Typography>
        </Stack>
      </Grid>
      <Grid item xs={12} sx={{ mt: 4 }}>
        <LoginForm />
      </Grid>
    </Grid>
  );
};
