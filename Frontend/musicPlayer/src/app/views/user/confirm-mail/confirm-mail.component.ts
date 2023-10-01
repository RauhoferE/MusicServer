import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-confirm-mail',
  templateUrl: './confirm-mail.component.html',
  styleUrls: ['./confirm-mail.component.scss']
})
export class ConfirmMailComponent implements OnInit {

  private email: string = '';

  private token: string = '';

  public failed: boolean = false;

  public loading: boolean = true;

  /**
   *
   */
  constructor(private authService: AuthenticationService, private route: ActivatedRoute, private message: NzMessageService) {
    

    if (!this.route.snapshot.paramMap.has('email') || !this.route.snapshot.paramMap.has('token')) {
      this.failed = true;
      return;
    }

    this.email = this.route.snapshot.paramMap.get('email') ?? '';
    this.token = this.route.snapshot.paramMap.get('token') ?? '';
  }

  async ngOnInit(): Promise<void> {
    if (this.failed) {
      this.loading = false;
      return;
    }

    this.authService.confirmEmail(this.email, this.token).subscribe({
      next: () => {
        this.failed = false;
        this.loading = false;
      },
      error: (error) => {
        this.failed = true;
        this.loading = false;
        this.message.error(error.error.message);
      },
      complete: () =>{
        this.loading = false;
      }
    })
  }

  

}
