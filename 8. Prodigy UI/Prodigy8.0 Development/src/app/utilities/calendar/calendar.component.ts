import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})

export class CalendarComponent implements OnInit {
  rangeDialog: boolean = false;
  today = new Date();
  selectedDate: Date;
  viewDate: Date = this.today;
  responsive: boolean = true;
  underline: boolean = false;
  constructor() { }

  ngOnInit(): void {

  }

  onDaySelect(day: Date): void {
    this.selectedDate = day;
  }
}