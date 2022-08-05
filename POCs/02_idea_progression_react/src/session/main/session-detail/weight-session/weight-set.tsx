import { atom, Atom, PrimitiveAtom, useAtom, useAtomValue } from "jotai";
import React, { useCallback, useEffect, useLayoutEffect, useRef } from "react";
import { setTypesAtom } from "../../../../exercises/exercises.atoms";
import { ExerciseSetVM, SetWeightMetrics } from "../../../shared/session.model";

import "../session-detail.css";

export type Props = {
    setAtom: PrimitiveAtom<ExerciseSetVM<SetWeightMetrics>>;
    isEditing: boolean;
    onEdit: () => void;
}

const selectedRowOptionsStyleAtom = atom<React.CSSProperties>({});
const weightIncrementAtom = atom(1);

const WeightSet: React.FC<Props> = ({setAtom, isEditing, onEdit}) => {
    const [set, setSet] = useAtom(setAtom);
    const [selectedRowOptionsStyle, setSelectedRowOptionsStyle] = useAtom(selectedRowOptionsStyleAtom);
    const [weightIncrement, setWeightIncrement] = useAtom(weightIncrementAtom);

    const availableSetTypes = useAtomValue(setTypesAtom);
    const setTypeOptions = availableSetTypes.map(setType => <option key={setType.id} value={setType.name}>{setType.name}</option>);

    const handleCompletedChange = (e: React.MouseEvent) => {
        let updatedSet = set;
        if(!set.actualMetrics)
            updatedSet = {...set, actualMetrics: {...set.targetMetrics}};

        setSet({...updatedSet, completed: !set.completed});
        e.stopPropagation();
    }

    const handleEdit = (e: React.MouseEvent) => {
        if(!set.actualMetrics)
            setSet({...set, actualMetrics: {...set.targetMetrics}});

        onEdit();
        e.stopPropagation();
    }

    const handleChangeSetType = (e: React.ChangeEvent<HTMLSelectElement>) => {
        setSet({...set, setType: e.currentTarget.value});
        e.stopPropagation();
    } 

    const handleChangeTargetMetric = (e: React.ChangeEvent<HTMLInputElement> | React.MouseEvent, metrics: Partial<SetWeightMetrics>) => {
        const setUpdate = {...set, targetMetrics: {...set.targetMetrics,...metrics}};
        if(setUpdate.targetMetrics.reps < 0)
            setUpdate.targetMetrics.reps = 0;

        setSet(setUpdate);
        e.stopPropagation();
    }

    const handleChangeActualMetric = (e: React.ChangeEvent<HTMLInputElement> | React.MouseEvent, metrics: Partial<SetWeightMetrics>) => {
        if(!set.actualMetrics)
            return;
        
        const setUpdate = {...set, actualMetrics: {...set.actualMetrics,...metrics}};

        if(setUpdate.actualMetrics.reps < 0)
            setUpdate.actualMetrics!.reps = 0;

        setSet(setUpdate);
        e.stopPropagation();
    }

    const handleSetWeightIncrement = (e: React.MouseEvent, increment: number) => {
        setWeightIncrement(increment);
        e.stopPropagation();
    }

    const completedCircleClass = set.completed ? 'bi-check-circle' : 'bi-circle';

    const selectedRowRef = useCallback((node: HTMLDivElement) => {
        if (node !== null) {
            const left = node.offsetLeft + node.offsetWidth + 5;
            setSelectedRowOptionsStyle({left});  
        }
      }, []);

    let row: JSX.Element;
    if(!isEditing)
        row =
            <div className={`row d-flex align-items-center align-self-center`} onClick={handleEdit}>
                <div className="col fw-bold">{set.setType}</div>
                <div className="col-2 text-center p-0">{set.targetMetrics.weight}</div>
                <div className="col-1 text-center p-0 me-3">{set.targetMetrics.reps}</div> 
                <div className="col-2 text-center p-0">{set.actualMetrics?.weight}</div>
                <div className="col-1 text-center p-0 me-3">{set.actualMetrics?.reps}</div>
                <div className="col-1 text-center">
                    <i className={`bi bi-check-circle ${completedCircleClass}`} onClick={handleCompletedChange}></i>
                </div>
            </div>
    else
        row =
            <>
                <div ref={selectedRowRef} className={`row d-flex align-items-center align-self-center selectedRow`}>
                    <div className="col d-flex text-center flex-column justify-content-center">
                        <i className="bi bi-arrow-up-circle invisible" style={{fontSize: "2rem", fontWeight: "bold"}}></i>
                        <select className="form-select" value={set.setType} onClick={(e) => e.stopPropagation()} onChange={handleChangeSetType}>
                            {setTypeOptions}
                        </select>
                        <i className="bi bi-arrow-down-circle invisible" style={{fontSize: "2rem", fontWeight: "bold"}}></i>
                    </div>
                    <div className="col-2 d-flex text-center flex-column justify-content-center">
                        <i className="bi bi-arrow-up-circle" onClick={(e) => handleChangeTargetMetric(e, {weight: set.targetMetrics.weight + weightIncrement })}></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center" 
                                value={set.targetMetrics.weight} 
                                onClick={(e) => e.stopPropagation()} 
                                onChange={(e) => handleChangeTargetMetric(e, {weight: +e.currentTarget.value })}></input>
                            <span className="input-group-text" id="basic-addon2">Kg</span>
                        </div>
                        <i className="bi bi-arrow-down-circle" onClick={(e) => handleChangeTargetMetric(e, {weight: set.targetMetrics.weight - weightIncrement })}></i>
                    </div>
                    <div className="col-1 d-flex text-center flex-column justify-content-center me-3">
                        <i className="bi bi-arrow-up-circle" onClick={(e) => handleChangeTargetMetric(e, {reps: ++set.targetMetrics.reps })}></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center reps"
                                value={set.targetMetrics.reps} 
                                onClick={(e) => e.stopPropagation()} 
                                onChange={(e) => handleChangeTargetMetric(e, {reps: +e.currentTarget.value })} />
                        </div>
                        <i className="bi bi-arrow-down-circle" onClick={(e) => handleChangeTargetMetric(e, {reps: --set.targetMetrics.reps })}></i>
                    </div>
                    <div className="col-2 d-flex text-center flex-column justify-content-center">
                        <i className="bi bi-arrow-up-circle" onClick={(e) => handleChangeActualMetric(e, {weight: set.actualMetrics!.weight + weightIncrement })}></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center" 
                                value={set.actualMetrics!.weight} 
                                onClick={(e) => e.stopPropagation()} 
                                onChange={(e) => handleChangeActualMetric(e, {weight: +e.currentTarget.value })}></input>
                            <span className="input-group-text" id="basic-addon2">Kg</span>
                        </div>
                        <i className="bi bi-arrow-down-circle" onClick={(e) => handleChangeActualMetric(e, {weight: set.actualMetrics!.weight - weightIncrement })}></i>
                    </div>
                    <div className="col-1 d-flex text-center flex-column justify-content-center me-3">
                        <i className="bi bi-arrow-up-circle" onClick={(e) => handleChangeActualMetric(e, {reps: ++set.actualMetrics!.reps })}></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center reps"
                                value={set.actualMetrics!.reps} 
                                onClick={(e) => e.stopPropagation()} 
                                onChange={(e) => handleChangeActualMetric(e, {reps: +e.currentTarget.value })} />
                        </div>
                        <i className="bi bi-arrow-down-circle" onClick={(e) => handleChangeActualMetric(e, {reps: --set.actualMetrics!.reps })}></i>
                    </div>
                    <div className="col-1 text-center">
                        <i className={`bi bi-check-circle ${completedCircleClass}`} onClick={handleCompletedChange}></i>
                    </div>
                    <div className="selectedRowOptions" style={selectedRowOptionsStyle}>
                        <div className={`circleWithText ${weightIncrement === 0.25 ? 'selected' : ''}`} onClick={(e) => handleSetWeightIncrement(e, 0.25)}>
                            <span>+.25</span> 
                        </div>
                        <div className={`circleWithText ${weightIncrement === 1 ? 'selected' : ''}`} onClick={(e) => handleSetWeightIncrement(e, 1)}>
                            <span>+1</span> 
                        </div>
                        <div className={`circleWithText ${weightIncrement === 5 ? 'selected' : ''}`} onClick={(e) => handleSetWeightIncrement(e, 5)}>
                            <span>+5</span> 
                        </div>
                    </div>
                </div>
            </>
    return row;
}

export default WeightSet;