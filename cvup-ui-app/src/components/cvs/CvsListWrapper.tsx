import { Box } from "@mui/material";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { CvsList } from "./CvsList";

export const CvsListWrapper = () => {
  const { cvsStore } = useStore();

  useEffect(() => {
    cvsStore.getCvsList();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <Box sx={{ marginTop: "4rem" }}>
      <CvsList />
    </Box>
  );
};
