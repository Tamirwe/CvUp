import { Box, Button } from "@mui/material";
import { useState } from "react";
import { observer } from "mobx-react";
import { useStore } from "../Hooks/useStore";
import { CandsSourceEnum } from "../models/GeneralEnums";
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
    const { candsStore, positionsStore } = useStore();
    const [positionsAdvancedOpen, setPositionsAdvancedOpen] = useState(false);
    const [showMatchMode, setShowMatchMode] = useState(false);

    return (
      <>
        {!showMatchMode && (
          <>
            <Box mt={1} mr={1} ml={1} sx={{ overflow: "hidden", display: "flex", alignItems: "center", gap: "0.5rem" }}>
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
                Find Matches
              </Button>
            </Box>
            <CandsList
              candsListData={candsPosList}
              candsSource={CandsSourceEnum.Position}
              advancedOpen={positionsAdvancedOpen}
              showAiDetails={true}
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
                Search Again
              </Button>
              <span style={{ fontSize: "0.85rem", color: "#888" }}>
                {`found ${candsStore.matchCandsPosList.length} matches cv's`}
              </span>
            </Box>
            <CandsList
              candsListData={candsStore.matchCandsPosList}
              candsSource={CandsSourceEnum.AllCands}
              advancedOpen={positionsAdvancedOpen}
              showAiDetails={true}
            />
          </>
        )}
      </>
    );
  },
);
