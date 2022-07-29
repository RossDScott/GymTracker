import { Component, OnInit } from '@angular/core';
import { ExerciseVM, SessionVM, ExerciseSetVM, SetMetrics, SetWeightMetrics, SetTimeMetrics } from './session.model';
import { SessionService } from './session.service';
import { DateTime, Duration } from "luxon";
import * as _ from 'lodash';
import { Chart, registerables } from 'chart.js';
Chart.register(...registerables);

@Component({
    selector: 'app-session',
    templateUrl: './session.component.html',
    styleUrls: ['./session.component.css']
})
export class SessionComponent implements OnInit {
    currentTime = DateTime.now();
    workoutTime: Duration = Duration.fromMillis(0);
    stopWatch: Duration | null = null;

    session: SessionVM | null = null;
    
    constructor(private sessionService: SessionService) { 
        
        setInterval(()=>{
            this.currentTime = DateTime.now();
            if(!this.session)
                return;

            this.workoutTime = DateTime.now().diff(this.session!.workoutStart, ['hours', 'minutes']);
        }, 1000);
    }

    ngOnInit(): void {
        this.session = this.sessionService.setupFakeSession();
        this.selectedExercise = this.session.exercises[0];
        console.dir(this.session)

        const labels = [
            '01/22',
            '02/22',
            '03/22',
            '04/22',
            '05/22',
            '06/22',
          ];
        
          const data = {
            labels: labels,
            datasets: [{
              label: 'Best Set Weight',
              backgroundColor: 'rgb(255, 99, 132)',
              borderColor: 'rgb(255, 99, 132)',
              data: [50, 51, 52, 50, 55, 55, 55],
            }]
          };
        
          const config = {
            type: 'line',
            data: data,
            options: {}
          }; 
          const myChart = new Chart(document.getElementById('myChart') as HTMLCanvasElement, config as any);


    }

    selectedExercise: ExerciseVM<SetMetrics> | null = null;
    handleSelectExercise(exercise: ExerciseVM<SetMetrics>){
        this.selectedExercise = exercise;
    }

    handleToggleComplete(exerciseSet: ExerciseSetVM<SetMetrics>){
        exerciseSet.completed = !exerciseSet.completed;
        if(exerciseSet.completed && !exerciseSet.actualMetrics)
            exerciseSet.actualMetrics = _.clone(exerciseSet.targetMetrics);

        if(!exerciseSet.completed)
            exerciseSet.actualMetrics = null;
    }
    editRow: ExerciseSetVM<SetMetrics> | null = null;
    handleEditRow(exerciseSet: ExerciseSetVM<SetMetrics>){
        this.selectedExercise?.sets.forEach(x => x.isEditing = false);
        exerciseSet.isEditing = true;
        this.editRow = exerciseSet;
    }
    handleEditConfirm(exerciseSet: ExerciseSetVM<SetMetrics>){
        const editRow = this.editRow;
        if(!editRow)
            return;

        _.merge(editRow, exerciseSet);
        editRow.completed = true;
        editRow.isEditing = false;
        this.editRow = null;
    }
    handleDelete(exerciseSet: ExerciseSetVM<SetMetrics>){
        const editRow = this.editRow;
        if(!editRow)
            return;

        _.pull(this.selectedExercise!.sets, editRow);
        this.editRow = null;
    }
    handleEditCancel(exerciseSet: ExerciseSetVM<SetMetrics>){
        const editRow = this.editRow;
        if(!editRow)
            return;

        editRow.isEditing = false;
        this.editRow = null;
    }
    handleAdd(exerciseSet: ExerciseSetVM<SetMetrics>){
        this.selectedExercise!.sets.push(exerciseSet);
        this.handleEditRow(exerciseSet);
    }

    stopWatchStartTime: DateTime | null = null;
    pausedStopWatchDuration: Duration | null = null;
    stopWatchStopped = false;
    stopWatchIntervalId: any;
    handleStartStopWatch(){
        this.stopWatchStartTime = DateTime.now();
        this.startStopWatch();
    }
    handleStopStopWatch(){
        clearInterval(this.stopWatchIntervalId);
        this.stopWatchStopped = true;
        this.pausedStopWatchDuration = this.stopWatch;
    }
    handleResumeStopWatch(){
        this.stopWatchStartTime = DateTime.now();
        this.startStopWatch();
        this.stopWatchStopped = false;
    }
    handleResetStopWatch(){
        clearInterval(this.stopWatchIntervalId);
        this.stopWatch = null;
        this.pausedStopWatchDuration = null;
        this.stopWatchStopped = false;
    }
    startStopWatch(){
        this.stopWatchIntervalId = setInterval(()=> {
            if(this.pausedStopWatchDuration){
                this.stopWatch = DateTime.now().plus(this.pausedStopWatchDuration).diff(this.stopWatchStartTime!, ['hours', 'minutes', 'seconds']);
                return;
            }

            this.stopWatch = DateTime.now().diff(this.stopWatchStartTime!, ['hours', 'minutes', 'seconds']);
        }, 5);
    } 
}
