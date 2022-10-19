import { createTheme } from "@mui/material/styles";

// assets
import colors from "../assets/scss/_themes-vars.module.scss";

// project imports
import componentStyleOverrides from "./CompStyleOverride";
import themePalette from "./Palette";
import themeTypography from "./Typography";

export type themeType = {
  colors?: { readonly [key: string]: string };
  heading: any;
  paper?: string;
  backgroundDefault?: string;
  background: any;
  darkTextPrimary: any;
  darkTextSecondary: any;
  textDark: any;
  menuSelected?: string;
  menuSelectedBack?: string;
  divider?: string;
  customization?: any;
};

export const theme = () => {
  const color = colors;

  const themeOption: themeType = {
    colors: color,
    heading: color.grey900,
    paper: color.paper,
    backgroundDefault: color.paper,
    background: color.primaryLight,
    darkTextPrimary: color.grey700,
    darkTextSecondary: color.grey500,
    textDark: color.grey900,
    menuSelected: color.secondaryDark,
    menuSelectedBack: color.secondaryLight,
    divider: color.grey200,
  };

  const themeOptions = {
    // direction: "ltr",
    palette: themePalette(themeOption),
    // mixins: {
    //   toolbar: {
    //     minHeight: "48px",
    //     padding: "16px",
    //     "@media (min-width: 600px)": {
    //       minHeight: "48px",
    //     },
    //   },
    // },
    typography: themeTypography(themeOption),
  };

  const themes = createTheme(themeOptions);

  // const themes = createTheme({
  //   direction: "ltr",
  //   mixins: {
  //     toolbar: {
  //       minHeight: "48px",
  //       padding: "16px",
  //       "@media (min-width: 600px)": {
  //         minHeight: "48px",
  //       },
  //     },
  //   },
  //   typography: themeTypography(themeOption),
  // });

  themes.components = componentStyleOverrides(themeOption);

  return themes;
};

export default theme;
