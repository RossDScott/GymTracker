import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ExerciseSetVM, ExerciseVM, SetTimeMetrics, SetType } from '../session.model';
import { SessionService } from '../session.service';
import * as _ from 'lodash';

@Component({
    selector: 'app-time-session',
    templateUrl: './time-session.component.html',
    styleUrls: ['./time-session.component.css', '../session.component.css']
})
export class TimeSessionComponent implements OnInit {

    @Input() exercise!: ExerciseVM<SetTimeMetrics>;

    @Output() onToggleComplete: EventEmitter<ExerciseSetVM<SetTimeMetrics>> = new EventEmitter<ExerciseSetVM<SetTimeMetrics>>();
    @Output() onEdit: EventEmitter<ExerciseSetVM<SetTimeMetrics>> = new EventEmitter<ExerciseSetVM<SetTimeMetrics>>();
    @Output() onEditConfirm: EventEmitter<ExerciseSetVM<SetTimeMetrics>> = new EventEmitter<ExerciseSetVM<SetTimeMetrics>>();
    @Output() onEditCancel: EventEmitter<ExerciseSetVM<SetTimeMetrics>> = new EventEmitter<ExerciseSetVM<SetTimeMetrics>>();
    @Output() onAdd: EventEmitter<ExerciseSetVM<SetTimeMetrics>> = new EventEmitter<ExerciseSetVM<SetTimeMetrics>>();
    @Output() onDelete: EventEmitter<ExerciseSetVM<SetTimeMetrics>> = new EventEmitter<ExerciseSetVM<SetTimeMetrics>>();

    setTypes: SetType[];

    constructor(sessionService: SessionService) { 
        this.setTypes = sessionService.fetchSetTypes();
    }

    ngOnInit(): void {

    }

    handleToggleComplete(event: Event, exerciseSet: ExerciseSetVM<SetTimeMetrics>){
        event.cancelBubble = true;
        this.onToggleComplete.emit(exerciseSet);
    }

    editRow: ExerciseSetVM<SetTimeMetrics> | null = null;
    handleStartEditRow(event: Event, exerciseSet: ExerciseSetVM<SetTimeMetrics>){
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
        const newRow: ExerciseSetVM<SetTimeMetrics> = {
            setType: lastRow?.setType || "",
            targetMetrics: lastRow?.targetMetrics || {timeMilliseconds: 0},
            actualMetrics: lastRow?.actualMetrics || {timeMilliseconds: 0},
            isEditing: true,
            completed: false
        };

        this.editRow = newRow;
        this.onAdd.emit(newRow);
    }
}
