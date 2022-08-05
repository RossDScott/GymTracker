import { atom, PrimitiveAtom, useAtom, useAtomValue } from 'jotai';
import { focusAtom } from 'jotai/optics';
import { splitAtom } from 'jotai/utils';
import React, { useMemo } from 'react';
import { selectedSetRowIndexAtom } from '../../../shared/session.atoms';
import { ExerciseSetVM, SelectedExercise, SetWeightMetrics } from '../../../shared/session.model';

import '../session-detail.css';
import WeightSet from './weight-set';

const WeightSession: React.FC<SelectedExercise> = ({selectedExerciseAtom}) => {
    const setsAtom = useMemo(() => focusAtom(selectedExerciseAtom, (optic) => optic.prop('sets')), [selectedExerciseAtom]);
    const setsAtomAtoms = splitAtom(setsAtom);

    const [selectedRowIndex, setSelectedRowIndex] = useAtom(selectedSetRowIndexAtom);
    const setAtoms = useAtomValue(setsAtomAtoms) as unknown as PrimitiveAtom<ExerciseSetVM<SetWeightMetrics>>[];
    const handleRowSelected =(index: number) => {
        setSelectedRowIndex(index);
    }
    const sets = setAtoms.map((set, index) => 
        <WeightSet key={index} setAtom={set} isEditing={selectedRowIndex === index} onEdit={() => handleRowSelected(index)}></WeightSet>
    );
    
    return (
        <div className="container-fluid">
            <div className="row fw-bold">
                <div className="col"></div>
                <div className="col-3 text-center ps-5 me-3">Target</div>
                <div className="col-3 text-center ps-5 me-3">Actual</div>
                <div className="col-1"></div>
            </div>
            <div className="row fw-bold">
                <div className="col"></div>
                <div className="col-2 text-center p-0">Weight</div>
                <div className="col-1 text-center p-0 me-3">Reps</div>
                <div className="col-2 text-center p-0">Weight</div>
                <div className="col-1 text-center p-0 me-3">Reps</div>
                <div className="col-1 text-center"></div>
            </div>
            {sets}
            {/* <div>
                {JSON.stringify(xx)}
            </div> */}
            {/* <div *ngFor="let exerciseSet of exercise.sets; let i=index" 
                className="row d-flex align-items-center align-self-center"
                [className.selectedRow]="exerciseSet.isEditing"
                (click)="handleStartEditRow($event, exerciseSet)">
                <ng-container *ngIf="!exerciseSet.isEditing">
                    <div className="col fw-bold">{{exerciseSet.setType}}</div>
                    <div className="col-2 text-center p-0">{{exerciseSet.targetMetrics.weight}}</div>
                    <div className="col-1 text-center p-0 me-3">{{exerciseSet.targetMetrics.reps}}</div> 
                    <div className="col-2 text-center p-0">{{exerciseSet.actualMetrics?.weight}}</div>
                    <div className="col-1 text-center p-0 me-3">{{exerciseSet.actualMetrics?.reps}}</div>
                    <div className="col-1 text-center" (click)="handleToggleComplete($event, exerciseSet)">
                        <i className="bi " [ngclassName]="exerciseSet.completed ? 'bi-check-circle' : 'bi-circle'" style="font-size: 1.5rem;"></i>
                    </div>
                </ng-container>
                <ng-container *ngIf="exerciseSet.isEditing">
                    <div className="col d-flex text-center flex-column justify-content-center">
                        <i className="bi bi-arrow-up-circle invisible" style="font-size: 2rem; font-weight: bold;"></i>
                        <select className="form-select" aria-label="Default select example" [(ngModel)]="editRow!.setType">
                            <option *ngFor="let setType of setTypes" [ngValue]="setType.name">{{setType.name}}</option>
                        </select>
                        <i className="bi bi-arrow-down-circle invisible" style="font-size: 2rem; font-weight: bold;"></i>
                    </div>
                    <div className="col-2 d-flex text-center flex-column justify-content-center">
                        <i className="bi bi-arrow-up-circle" style="font-size: 2rem; font-weight: bold;"></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center" [(ngModel)]="editRow!.targetMetrics.weight">
                            <span className="input-group-text" id="basic-addon2">Kg</span>
                        </div>
                        <i className="bi bi-arrow-down-circle" style="font-size: 2rem; font-weight: bold;"></i>
                    </div>
                    <div className="col-1 d-flex text-center flex-column justify-content-center me-3">
                        <i className="bi bi-arrow-up-circle" style="font-size: 2rem; font-weight: bold;"></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center reps" [(ngModel)]="editRow!.targetMetrics.reps">
                        </div>
                        <i className="bi bi-arrow-down-circle" style="font-size: 2rem; font-weight: bold;"></i>
                    </div>
                    <div className="col-2 d-flex text-center flex-column justify-content-center">
                        <i className="bi bi-arrow-up-circle" style="font-size: 2rem; font-weight: bold;"></i>
                        <div className="input-group" >
                            <input type="text" className="form-control text-center" [(ngModel)]="editRow!.actualMetrics!.weight">
                            <span className="input-group-text" id="basic-addon2">Kg</span>
                        </div>
                        <i className="bi bi-arrow-down-circle" style="font-size: 2rem; font-weight: bold;"></i>
                    </div>
                    <div className="col-1 d-flex text-center flex-column justify-content-center me-3">
                        <i className="bi bi-arrow-up-circle" style="font-size: 2rem; font-weight: bold;"></i>
                        <div className="input-group">
                            <input type="text" className="form-control text-center reps" [(ngModel)]="editRow!.actualMetrics!.reps">
                        </div>
                        <i className="bi bi-arrow-down-circle" style="font-size: 2rem; font-weight: bold;"></i>
                    </div>
                    <div className="col-1 d-flex flex-column align-items-center align-self-center">
                        <i className="bi bi-trash" (click)="handleDeleteEditRow($event)"></i>
                        <i className="bi bi-x-circle" (click)="handleCancelEditRow($event)"></i>
                        <i className="bi bi-check-circle" (click)="handleConfirmEditRow($event)"></i>
                    </div> 
                </ng-container>
            </div> */}
            <div className="row">
                <div className="col-11"></div>
                <div className="col text-center">
                    <i className="bi bi-plus-circle-fill"></i>
                </div>
            </div>
        </div>
    )
}

export default WeightSession;