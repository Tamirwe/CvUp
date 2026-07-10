import { Box, Button } from "@mui/material";
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
  const [showMatchMode, setShowMatchMode] = useState(false);

  useEffect(() => {
    setList(candsStore.posCandsList);
  }, [candsStore.posCandsList]);

  useEffect(() => {
    setShowMatchMode(false);
  }, [positionsStore.selectedPosition?.id]);

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
      {!showMatchMode && (
        <>
          <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden", display: "flex", alignItems: "center", gap: "0.5rem" }}>
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
            <Button
              variant="outlined"
              size="small"
              sx={{ whiteSpace: "nowrap", px: 2 }}
              onClick={() => {
                if (positionsStore.selectedPosition?.id) {
                  candsStore.findPositionMatchCvs(
                    positionsStore.selectedPosition.id,
                  );
                  setShowMatchMode(true);
                }
              }}
            >
              XX
            </Button>
          </Box>
          <CandsList
            candsListData={list}
            candsSource={CandsSourceEnum.Position}
            advancedOpen={positionsAdvancedOpen}

          />
        </>
      )}
      {showMatchMode && (
        <>
          <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden", display: "flex", alignItems: "center", gap: "0.5rem" }}>
            <Button
              variant="outlined"
              size="small"
              sx={{ whiteSpace: "nowrap", px: 2 }}
              onClick={() => setShowMatchMode(false)}
            >
              Back
            </Button>
            <Button
              variant="outlined"
              size="small"
              sx={{ whiteSpace: "nowrap", px: 2 }}
              onClick={() => {
                if (positionsStore.selectedPosition?.id) {
                  candsStore.findPositionMatchCvs(
                    positionsStore.selectedPosition.id,
                  );
                }
              }}
            >
              Search Matches CV's
            </Button>
            <span style={{ fontSize: "0.85rem", color: "#888" }}>
              {`found ${candsStore.matchCandsPosList.length} matches cv's`}
            </span>
          </Box>
          <CandsList
            candsListData={candsStore.matchCandsPosList}
            candsSource={CandsSourceEnum.AllCands}
            advancedOpen={positionsAdvancedOpen}

          />
        </>
      )}
    </>
  );
});
