import { observer } from "mobx-react";
import { useEffect } from "react";
import { useStore } from "../../Hooks/useStore";
import { FoldersList } from "./FoldersList";
import { Box } from "@mui/material";
import { SearchControl } from "../header/SearchControl";

export const FoldersListWrapper = () => {
  const handleFoldersSearch = (val: string) => {};

  return (
    <Box>
      <Box mt={1} ml={1}>
        <SearchControl onSearch={handleFoldersSearch} />
      </Box>
      <FoldersList />;
    </Box>
  );
};
