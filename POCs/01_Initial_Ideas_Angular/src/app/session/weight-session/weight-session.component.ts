import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ExerciseSetVM, ExerciseVM, SetType, SetWeightMetrics } from '../session.model';
import { SessionService } from '../session.service';
import * as _ from 'lodash';

@Component({
    selector: 'app-weight-session',
    templateUrl: './weight-session.component.html',
    styleUrls: ['./weight-session.component.css', '../session.component.css']
})
export class WeightSessionComponent implements OnInit {

    @Input() exercise!: ExerciseVM<SetWeightMetrics>;

    @Output() onToggleComplete: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();
    @Output() onEdit: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();
    @Output() onEditConfirm: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();
    @Output() onEditCancel: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();
    @Output() onAdd: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();
    @Output() onDelete: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();

    setTypes: SetType[];

    constructor(sessionService: SessionService) { 
        this.setTypes = sessionService.fetchSetTypes();
    }

    ngOnInit(): void {

    }

    handleToggleComplete(event: Event, exerciseSet: ExerciseSetVM<SetWeightMetrics>){
        event.cancelBubble = true;
        this.onToggleComplete.emit(exerciseSet);
    }

    editRow: ExerciseSetVM<SetWeightMetrics> | null = null;
    handleStartEditRow(event: Event, exerciseSet: ExerciseSetVM<SetWeightMetrics>){
        event.cancelBubble = true;

        if(this.editRow)
            return;

        this.editRow = _.cloneDeep(exerciseSet);
        if(!this.editRow.actualMetrics)
            this.editRow.actualMetrics = _.clone(this.editRow!.targetMetrics);
        
        this.onEdit.emit(exerciseSet);
        console.dir(exerciseSet)
    }

    handleConfirmEditRow(event: Event){
        event.cancelBubble = true;

        const editRow = this.editRow;
        if(!editRow)
            return;

        this.onEditConfirm.emit(editRow);
        this.editRow = null;
    }

    handleDeleteEditRow(event: Event){
        event.cancelBubble = true;

        const editRow = this.editRow;
        if(!editRow)
            return;

        this.onDelete.emit(editRow);
        this.editRow = null; 
    }

    handleCancelEditRow(event: Event){
        event.cancelBubble = true;

        const editRow = this.editRow;
        if(!editRow)
            return;

        this.onEditCancel.emit(editRow);
        this.editRow = null;
    }

    handleAddRow() {
        const lastRow = _.last(this.exercise.sets);
        const newRow: ExerciseSetVM<SetWeightMetrics> = {
            setType: lastRow?.setType || "",
            targetMetrics: lastRow?.targetMetrics || {weight: 0, reps: 0},
            actualMetrics: lastRow?.actualMetrics || {weight: 0, reps: 0},
            isEditing: true,
            completed: false
        };

        this.editRow = newRow;
        this.onAdd.emit(newRow);
    }
}
