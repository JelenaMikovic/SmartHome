import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';

@Component({
  selector: 'app-chang-password',
  templateUrl: './chang-password.component.html',
  styleUrls: ['./chang-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  codeForm = new FormGroup({
    old: new FormControl('', [Validators.required]),
    new: new FormControl('', [Validators.required]),
  })


  constructor(
    public dialogRef: MatDialogRef<ChangePasswordComponent>,
    private snackBar: MatSnackBar,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
  }

  close() {
    this.dialogRef.close();
  }

  activate(){
    const dto = {
      oldPassword: this.codeForm.get('old')?.value,
      newPassword: this.codeForm.get('new')?.value
    }
    
    this.authService.changePassword(dto).subscribe(
      (success) => {
        console.log(success)
          this.router.navigate(['/home']);
      },
      (error) => {
        this.snackBar.open(error.error, 'Close', { duration: 3000 });
      }
    );
  }

}
