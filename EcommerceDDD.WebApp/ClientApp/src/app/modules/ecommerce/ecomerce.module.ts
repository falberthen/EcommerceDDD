import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './components/home/home.component';
import { SharedModule } from 'app/shared/shared.module';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { HistoryViewerComponent } from './components/history-viewer/history-viewer.component';

@NgModule({
   declarations: [
      HomeComponent,
      HistoryViewerComponent
   ],
   imports: [
      CommonModule,
      SharedModule,
      NgbDatepickerModule
   ],
   providers: []
})
export class EcommerceModule { }
