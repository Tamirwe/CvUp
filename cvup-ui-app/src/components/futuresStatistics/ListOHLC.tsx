import { useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { observer } from "mobx-react";
import styles from "./ListOHLC.module.scss";
import { Iohlc } from "../../models/FuStatModel";

interface IProps {
  onEdit: (ohlcRow: Iohlc) => void;
}

export const ListOHLC = observer(({ onEdit }: IProps) => {
  const { futuresStatisticStore } = useStore();
  const [isShowAllCols, setIsShowAllCols] = useState(false);

  const handleEditRow = (ohlcRow: Iohlc) => {
    onEdit(ohlcRow);
  };

  const handleDeleteRow = (id: number) => {
    futuresStatisticStore.deleteDayOhlc(id);
  };

  return (
    <div className={styles.wrapper}>
      <button
        style={{ width: "9rem" }}
        data-color={"edit"}
        onClick={() => {
          setIsShowAllCols(!isShowAllCols);
        }}
      >
        Show All Columns
      </button>
      <div className={styles.tableWrapper}>
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>OPEN</th>
              <th>HIGH</th>
              <th>LOW</th>
              <th>CLOSE</th>
              <th>Points</th>
              {isShowAllCols && <th>Overlap High</th>}
              {isShowAllCols && <th>Overlap Low</th>}
              <th>
                Overlap
                <br />
                Points/Percent
              </th>
            </tr>
          </thead>
          <tbody>
            {futuresStatisticStore.dailyStatList?.map((item, i) => {
              return (
                <TableRow
                  item={item}
                  onEdit={handleEditRow}
                  onDelete={handleDeleteRow}
                  key={item.id}
                  isShowAllCols={isShowAllCols}
                />
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
});

interface ITableRowProps {
  item: Iohlc;
  isShowAllCols: boolean;
  onEdit: (ohlcRow: Iohlc) => void;
  onDelete: (id: number) => void;
}

const TableRow = ({
  item,
  onEdit,
  onDelete,
  isShowAllCols = false,
}: ITableRowProps) => {
  const [isHovered, setIsHovered] = useState(false);

  return (
    <tr
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      <td>
        {item.statisticDate &&
          new Date(item.statisticDate).toLocaleDateString("en-US", {
            year: "numeric",
            day: "2-digit",
            month: "short",
          })}
        {isHovered && (
          <div className={styles.deleteButtonWrapper}>
            <div>
              <button data-color={"edit"} onClick={() => onEdit(item)}>
                Edit
              </button>
              <button
                data-color={"delete"}
                onClick={() => onDelete(item.id || 0)}
              >
                Delete
              </button>
            </div>
          </div>
        )}
      </td>
      <td>{item.open}</td>
      <td>{item.high}</td>
      <td>{item.low}</td>
      <td>{item.close}</td>
      <td>{item.dayPoints}</td>
      {isShowAllCols && <td>{item.highOverlap}</td>}
      {isShowAllCols && <td>{item.lowOverlap}</td>}
      <td>
        {item.overlapPoints} / {item.overlapPercent}%
      </td>
    </tr>
  );
};
