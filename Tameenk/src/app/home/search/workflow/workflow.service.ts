import { Injectable } from '@angular/core';
import { STEPS } from './workflow.model';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
    private workflow = ['main', 'insured'];
    firstStep = 'main';
    currentStep = 'main';
    lastStep = this.workflow[this.workflow.length - 1];
    getNextStep(): string {
        if(this.currentStep !== this.lastStep) {
            return this.currentStep = this.workflow[this.workflow.indexOf(this.currentStep) + 1];
        }
    }
    getPrevStep(): string {
        if(this.currentStep !== this.firstStep) {
            return this.currentStep = this.workflow[this.workflow.indexOf(this.currentStep) - 1];
        }
    }
    removeStep(step) {
        this.workflow.splice(this.workflow.indexOf(step), 1);
        this.lastStep = this.workflow[this.workflow.length - 1];
    }
    refreshSteps() {
        this.workflow = ['main', 'insured'];
        this.lastStep = this.workflow[this.workflow.length - 1];
    }
}
