import java.util.Scanner;
class PrintCircleV3{
    public static void main(String args[]){
        int radius = takeInput();

        int x1 = radius;
        int y1 = radius;

        boolean buffer[][] = new boolean[radius*2+1][radius*2+1];

        fillBuffer(buffer, x1, y1, radius);
        //fillBufferwithFunction(buffer, x1, y1, radius);
        printBuffer(buffer);        
    }

    private static void fillBuffer(boolean buffer[][], int x1, int y1, int radius)
    {
        for (float angle = 0; angle < Math.PI * 2; angle+=0.05)
        {
            int x = (int)(x1 + Math.cos(angle) * radius);
            int y = (int)(y1 + Math.sin(angle) * radius);
            buffer[x][y] = true;
        }
    }
    
    private static void fillBufferwithFunction(boolean buffer[][], int x1, int y1, int radius)
    {
        for (float angle = 0; angle < Math.PI * 2; angle+=0.01)
        {
            int x = (int)(x1 + Math.sin(angle*2) * radius);
            int y = (int)(y1 + Math.sin(angle*3) * radius);
            buffer[y][x] = true;
        }
    }
    
    private static void printBuffer(boolean buffer[][])
    {
        for (int x = 0; x < buffer.length; x++)
        {
            for (int y = 0; y < buffer[x].length; y++)
            {
                if (buffer[x][y])
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