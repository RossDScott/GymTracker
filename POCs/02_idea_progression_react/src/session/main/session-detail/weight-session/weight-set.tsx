import { Atom, PrimitiveAtom, useAtom } from "jotai";
import React from "react";
import { ExerciseSetVM, SetWeightMetrics } from "../../../shared/session.model";

export type Props = {
    setAtom: PrimitiveAtom<ExerciseSetVM<SetWeightMetrics>>;
    isEditing: boolean;
    onEdit: () => void;
}

const WeightSet: React.FC<Props> = ({setAtom, isEditing, onEdit}) => {
    const [set, setSet] = useAtom(setAtom);

    const handleCompletedChange = (e: React.MouseEvent) => {
        setSet({...set, completed: !set.completed});
        e.stopPropagation();
    }

    const completedCircleClass = set.completed ? 'bi-check-circle' : 'bi-circle';

    let row: JSX.Element;
    if(!isEditing)
        row =
            <div className={`row d-flex align-items-center align-self-center`} onClick={onEdit}>
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
            <div className={`row d-flex align-items-center align-self-center selectedRow`}>
                <div className="col d-flex text-center flex-column justify-content-center">
                    <i className="bi bi-arrow-up-circle invisible" style={{fontSize: "2rem", fontWeight: "bold"}}></i>
                    {/* <select className="form-select" aria-label="Default select example" [(ngModel)]="editRow!.setType">
                        <option *ngFor="let setType of setTypes" [ngValue]="setType.name">{{setType.name}}</option>
                    </select> */}
                    <i className="bi bi-arrow-down-circle invisible" style={{fontSize: "2rem", fontWeight: "bold"}}></i>
                </div>
                <div className="col-2 d-flex text-center flex-column justify-content-center">
                    <i className="bi bi-arrow-up-circle"></i>
                    <div className="input-group">
                        <input type="text" className="form-control text-center" value={set.targetMetrics.weight} onChange={(e) => setSet({...set, targetMetrics: {...set.targetMetrics, weight: +e.currentTarget.value }})}></input>
                        <span className="input-group-text" id="basic-addon2">Kg</span>
                    </div>
                    <i className="bi bi-arrow-down-circle"></i>
                </div>
                <div className="col-1 text-center p-0 me-3">{set.targetMetrics.reps}</div> 
                <div className="col-2 text-center p-0">{set.actualMetrics?.weight}</div>
                <div className="col-1 text-center p-0 me-3">{set.actualMetrics?.reps}</div>
                <div className="col-1 text-center">
                    <i className={`bi bi-check-circle ${completedCircleClass}`} onClick={handleCompletedChange}></i>
                </div>
            </div>
    
    return row;
}

export default WeightSet;