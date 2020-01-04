import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html'
})
export class LogoutComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
    localStorage.removeItem('currentUser');
    //this.router.navigate(["/giris"]);
    window.location.href = window.location.origin + '/giris';
  }

}
