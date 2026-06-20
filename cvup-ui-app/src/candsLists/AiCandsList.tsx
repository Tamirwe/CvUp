import { Box } from "@mui/material";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { AiList } from "../components/cands/AiSearch/AiList";

interface IProps {
  onSearch: (searchVals: ISearchModel) => void;
}

export const AiCandsList = observer(({ onSearch }: IProps) => {
  const { candsStore } = useStore();

  return (
    <>
      <Box mt={1} mr={1} ml={1}>
        <SearchControl onSearch={onSearch} />
      </Box>
      <AiList
        candsListData={candsStore.aiCandsResults}
        candsSource={CandsSourceEnum.AI}
      />
    </>
  );
});
