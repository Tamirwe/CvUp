import { Box } from "@mui/material";
import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ICand, ISearchModel } from "../models/GeneralModels";
import { sortCandList } from "../utils/GeneralUtils";
import { SearchControl } from "../components/header/SearchControl";
import { CandsList } from "../components/cands/CandsList";

export const PositionCandsList = observer(() => {
  const { candsStore, positionsStore } = useStore();
  const [list, setList] = useState<ICand[]>([]);
  const [positionsAdvancedOpen, setPositionsAdvancedOpen] = useState(false);

  useEffect(() => {
    setList(candsStore.posCandsList);
  }, [candsStore.posCandsList]);

  const sortList = (isDesc: boolean) => {
    setList(sortCandList(isDesc, candsStore.posCandsList));
  };

  const handleSearch = (searchVals: ISearchModel) => {
    if (searchVals.value) {
      candsStore.searchPositionCands(searchVals);
    } else if (positionsStore.selectedPosition?.id) {
      candsStore.getPositionCandsList(positionsStore.selectedPosition.id);
      candsStore.setDisplayCandOntopPCList();
    }
  };

  return (
      <>
        <Box
          mt={1}
          mr={1}
          ml={1}
          sx={{
            overflow: "hidden",
            display: "flex",
            alignItems: "center",
            gap: "0.5rem",
          }}
        >
          <SearchControl
            onSearch={handleSearch}
            onShowAdvanced={() =>
              setPositionsAdvancedOpen(!positionsAdvancedOpen)
            }
            shoeAdvancedIcon={true}
            records={candsStore.posCandsList && candsStore.posCandsList.length}
            showSort={true}
            onSort={sortList}
            showRefreshList={true}
            showSE={true}
            positionId={positionsStore.selectedPosition?.id || 0}
          />
        </Box>
        <CandsList
          candsListData={list}
          candsSource={CandsSourceEnum.Position}
          advancedOpen={positionsAdvancedOpen}
        />
      </>
  );
});
