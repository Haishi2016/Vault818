import java.util.Scanner;
class PrintCircleV1{
    public static void main(String args[]){
        int radius = takeInput();

        int x1 = radius;
        int y1 = radius;

        for (int y = y1-radius; y <= y1+radius; y++)
        {
            for (int x = x1-radius; x <= x1+radius; x++)
            {
                if (Math.abs((x-x1)*(x-x1) + (y-y1)*(y-y1) - radius*radius) < radius)
                    System.out.print('*');
                else
                    System.out.print(' ');
            }
            System.out.println();
        }
    }
    private static int takeInput()
    {
        try (Scanner scanner = new Scanner(System.in))
        {
            System.out.print("Please enter a radius: ");
            while (!scanner.hasNextInt()){
                System.out.print("Please enter a radius: ");
                scanner.next();
            }
            return scanner.nextInt();
        }
    }
}