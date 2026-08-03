module.exports = {
  root: true,
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaVersion: "latest",
    sourceType: "module",
    ecmaFeatures: { jsx: true },
  },
  env: {
    browser: true,
    es2021: true,
  },
  plugins: ["mobx", "react-hooks"],
  extends: ["plugin:mobx/recommended", "plugin:react-hooks/recommended"],
  rules: {
    "mobx/missing-observer": "off",
  },
  ignorePatterns: ["build", "node_modules", "vite.config.ts"],
};
