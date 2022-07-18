import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ExerciseSetVM, ExerciseVM, SetWeightMetrics } from '../session.model';

@Component({
    selector: 'app-weight-session',
    templateUrl: './weight-session.component.html',
    styleUrls: ['./weight-session.component.css', '../session.component.css']
})
export class WeightSessionComponent implements OnInit {

    @Input() exercise!: ExerciseVM<SetWeightMetrics>;

    @Output() onToggleComplete: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();
    @Output() onEdit: EventEmitter<ExerciseSetVM<SetWeightMetrics>> = new EventEmitter<ExerciseSetVM<SetWeightMetrics>>();

    constructor() { }

    ngOnInit(): void {

    }

    editRow: ExerciseSetVM<SetWeightMetrics> | null = null;
    handleStartEditRow(exerciseSet: ExerciseSetVM<SetWeightMetrics>, event: Event){
        event.cancelBubble = true;
        this.editRow = Object.assign({}, exerciseSet);
        this.onEdit.emit(exerciseSet);
    }

}
