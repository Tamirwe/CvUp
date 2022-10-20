import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  IconButton,
  InputAdornment,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { Link } from "react-router-dom";

export const LoginForm = () => {
  const [isShowPassword, setIsShowPassword] = useState(false);

  const handlePasswordChange = () => {};
  const handleUserNameChange = () => {};

  return (
    <form noValidate>
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
      <Stack
        direction="row"
        alignItems="center"
        justifyContent="space-between"
        spacing={1}
      >
        <FormControlLabel
          control={
            <Checkbox
              // checked={checked}
              // onChange={(event) => setChecked(event.target.checked)}
              name="checked"
              color="primary"
            />
          }
          label="Keep me sign in"
        />
        <Typography component={Link} to="/forgot-password" variant="subtitle2">
          Forgot Password?
        </Typography>
      </Stack>
      <Box sx={{ mt: 4 }}>
        <Button
          // disableElevation
          // disabled={isSubmitting}
          fullWidth
          size="large"
          type="submit"
          variant="contained"
          color="secondary"
        >
          Sign In
        </Button>
      </Box>
    </form>
  );
};
