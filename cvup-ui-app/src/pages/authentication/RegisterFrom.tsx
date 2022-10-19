import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  Grid,
  IconButton,
  InputAdornment,
  TextField,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link } from "react-router-dom";

interface props {}

export const RegisterForm = ({}: props) => {
  const theme = useTheme();
  const matchDownSM = useMediaQuery(theme.breakpoints.down("md"));
  const [isShowPassword, setIsShowPassword] = useState(false);

  const handlePasswordChange = () => {};
  const handleUserNameChange = () => {};
  const handleCompanyChange = () => {};

  return (
    <form noValidate>
      <Grid container spacing={matchDownSM ? 0 : 2}>
        <Grid item xs={12} sm={6}>
          <TextField
            fullWidth
            required
            margin="normal"
            type="text"
            id="lastNameTxt"
            label="First Name"
            variant="outlined"
            onChange={handleCompanyChange}
          />
        </Grid>
        <Grid item xs={12} sm={6}>
          <TextField
            fullWidth
            required
            margin="normal"
            type="text"
            id="firstNameTxt"
            label="Last Name"
            variant="outlined"
            onChange={handleCompanyChange}
          />
        </Grid>
      </Grid>
      <TextField
        fullWidth
        required
        margin="normal"
        type="text"
        id="companyNameTxt"
        label="Company"
        variant="outlined"
        onChange={handleCompanyChange}
      />
      <TextField
        fullWidth
        required
        margin="normal"
        type="text"
        id="userNameTxt"
        label="Email Address / Username"
        variant="outlined"
        onChange={handleUserNameChange}
      />
      <TextField
        fullWidth
        required
        margin="normal"
        type={isShowPassword ? "text" : "password"}
        id="passwordTxt"
        label="Password"
        variant="outlined"
        onChange={handlePasswordChange}
        InputProps={{
          endAdornment: (
            <InputAdornment position="end">
              <IconButton
                aria-label="toggle password visibility"
                onClick={() => setIsShowPassword(!isShowPassword)}
                edge="end"
              >
                {isShowPassword ? (
                  <MdOutlineVisibilityOff />
                ) : (
                  <MdOutlineVisibility />
                )}
              </IconButton>
            </InputAdornment>
          ),
        }}
      />
      <FormControlLabel
        control={<Checkbox />}
        label={
          <Typography variant="subtitle1">
            Agree with &nbsp;
            <Typography variant="subtitle2" component={Link} to="/terms">
              Terms & Condition.
            </Typography>
          </Typography>
        }
      />
      <Box sx={{ mt: 2 }}>
        <Button
          // disableElevation
          // disabled={isSubmitting}
          fullWidth
          size="large"
          type="submit"
          variant="contained"
          color="secondary"
        >
          Sign up
        </Button>
      </Box>
    </form>
  );
};
