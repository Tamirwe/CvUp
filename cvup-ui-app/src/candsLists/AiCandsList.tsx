import { Box } from "@mui/material";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

export const AiCandsList = observer(() => {
  const { candsStore } = useStore();

  const handleSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.AiSearchCands(searchVals);
    }
  };

  return (
    <>
      <Box mt={1} mr={1} ml={1}>
        <SearchControl onSearch={handleSearch} />
      </Box>
      <CandsList
        candsListData={candsStore.aiCandsResults}
        candsSource={CandsSourceEnum.AI}
      />
    </>
  );
});
