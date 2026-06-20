import { Box } from "@mui/material";
import { useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum, TabsCandsEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

interface IProps {
  candsPosList: ICand[];
  onSearch: (searchVals: ISearchModel) => void;
  onSort: (isDesc: boolean) => void;
}

export const PositionCandsList = observer(
  ({ candsPosList, onSearch, onSort }: IProps) => {
    const { candsStore } = useStore();
    const [positionsAdvancedOpen, setPositionsAdvancedOpen] = useState(false);

    return (
      <>
        <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden" }}>
          <SearchControl
            onSearch={onSearch}
            onShowAdvanced={() =>
              setPositionsAdvancedOpen(!positionsAdvancedOpen)
            }
            shoeAdvancedIcon={true}
            records={candsStore.posCandsList && candsStore.posCandsList.length}
            showSort={true}
            onSort={onSort}
            showRefreshList={true}
          />
        </Box>
        <CandsList
          candsListData={candsPosList}
          candsSource={CandsSourceEnum.Position}
          advancedOpen={positionsAdvancedOpen}
        />
      </>
    );
  },
);
