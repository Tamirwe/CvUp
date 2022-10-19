import { ThemeProvider } from "@emotion/react";
import { StyledEngineProvider, CssBaseline } from "@mui/material";
import themes from ".";

interface props {
  children: React.ReactNode;
}

export const ThemeCustomization = ({ children }: props) => {
  return (
    <StyledEngineProvider injectFirst>
      <ThemeProvider theme={themes}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </StyledEngineProvider>
  );
};
