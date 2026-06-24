import { Box } from "@mui/material";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { AiList } from "../components/cands/AiSearch/AiList";

export const AiCandsList = observer(() => {
  const { candsStore } = useStore();

  const handleSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.AiSearchCands(searchVals.value);
    }
  };

  return (
    <>
      <Box mt={1} mr={1} ml={1}>
        <SearchControl onSearch={handleSearch} />
      </Box>
      <AiList
        candsListData={candsStore.aiCandsResults}
        candsSource={CandsSourceEnum.AI}
      />
    </>
  );
});
