import { observer } from "mobx-react";
import { PositionsList } from "./PositionsList";
import { Box } from "@mui/material";
import { SearchControl } from "../header/SearchControl";

export const PositionsListWrapper = () => {
  const handlePositionsSearch = (val: string) => {};

  return (
    <Box>
      <Box mt={1} ml={1}>
        <SearchControl onSearch={handlePositionsSearch} />
      </Box>
      <PositionsList />;
    </Box>
  );
};
